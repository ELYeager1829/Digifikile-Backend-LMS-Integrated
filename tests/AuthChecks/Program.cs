using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using DigiFikileLms.API.Controllers;
using DigiFikileLms.API.Extensions;
using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.DTOs.Complaint;
using DigiFikileLms.Application.Features.Auth.Commands;
using DigiFikileLms.Application.Features.Complaints.Commands;
using DigiFikileLms.Application.Features.Complaints.Queries;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Common;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Extensions;
using DigiFikileLms.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Configuration.AddInMemoryCollection(new Dictionary<string,string?> {
    ["Jwt:Issuer"]="AuthChecks", ["Jwt:Audience"]="AuthChecks",
    ["Jwt:SecretKey"]=Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(48))
});
var users = new List<UserAccount>();
var students = new List<Student>();
var hasher = new PasswordHasherService();
var tokens = new TokenService(builder.Configuration);
var userRepo = Fake.Create<IUserAccountRepository>((method, args) => method switch {
    "GetByEmailAsync" => Task.FromResult(users.SingleOrDefault(u => u.Email.Equals(((string)args[0]!).Trim(), StringComparison.OrdinalIgnoreCase))),
    "GetByIdAsync" => Task.FromResult(users.SingleOrDefault(u => u.Id == (int)args[0]!)),
    "GetWithDetailsAsync" => Task.FromResult(users.SingleOrDefault(u => u.Id == (int)args[0]!)),
    "ExistsByEmailAsync" => Task.FromResult(users.Any(u => u.Email.Equals(((string)args[0]!).Trim(), StringComparison.OrdinalIgnoreCase))),
    "AddAsync" => AddUser((UserAccount)args[0]!),
    "SaveChangesAsync" => Task.FromResult(1),
    _ => throw new NotSupportedException(method)
});
Task AddUser(UserAccount user) {
    typeof(UserAccount).GetProperty("Id")!.SetValue(user, users.Count + 1);
    users.Add(user); return Task.CompletedTask;
}
var studentRepo = Fake.Create<IStudentRepository>((method, args) => method switch {
    "GetByStudentNumberAsync" => Task.FromResult(students.SingleOrDefault(s => s.StudentNumber == (string)args[0]!)),
    "AddAsync" => AddStudent((Student)args[0]!),
    "SaveChangesAsync" => Task.FromResult(1),
    _ => throw new NotSupportedException(method)
});
Task AddStudent(Student student) {
    typeof(Student).GetProperty("Id")!.SetValue(student, students.Count + 1);
    typeof(UserAccount).GetProperty("Student")!.SetValue(student.User, student);
    students.Add(student); return Task.CompletedTask;
}
builder.Services.AddSingleton(userRepo);
builder.Services.AddSingleton(studentRepo);
builder.Services.AddSingleton<IPasswordHasher>(hasher);
builder.Services.AddSingleton<ITokenService>(tokens);
// Two-step login fixture: the real OtpService is supplied by the OTP/EmailJS owner. Here the
// PIN is fixed so the endpoints can be exercised end to end.
const string OtpCode = "123456";
var otpService = Fake.Create<IOtpService>((method, args) => method switch {
    "SendLoginOtpAsync" => Task.FromResult(10),
    "VerifyLoginOtpAsync" => Task.FromResult((string)args[1]! == OtpCode),
    _ => throw new NotSupportedException(method)
});
// Complaints: every signed-in role lays a complaint; the System Administrator sees them all and
// works through them; nobody can read or change someone else's complaint.
var complaints = new List<Complaint>();
var complaintRepo = Fake.Create<IComplaintRepository>((method, args) => method switch {
    "GetByIdAsync" => Task.FromResult(complaints.SingleOrDefault(c => c.Id == (int)args[0]!)),
    "GetAllAsync" => Task.FromResult(complaints
        .Where(c => args[0] == null || c.Status == (ComplaintStatus)args[0]!)
        .OrderByDescending(c => c.CreatedAt).AsEnumerable()),
    "GetByComplainantAsync" => Task.FromResult(complaints
        .Where(c => c.ComplainantUserId == (int)args[0]!)
        .OrderByDescending(c => c.CreatedAt).AsEnumerable()),
    "AddAsync" => AddComplaint((Complaint)args[0]!),
    "SaveChangesAsync" => Task.FromResult(1),
    _ => throw new NotSupportedException(method)
});
Task AddComplaint(Complaint complaint) {
    typeof(Complaint).GetProperty("Id")!.SetValue(complaint, complaints.Count + 1);
    typeof(BaseEntity).GetProperty("CreatedAt")!.SetValue(complaint, DateTime.UtcNow.AddSeconds(complaints.Count));
    // The read paths map the complainant through the navigation, so it has to be populated here.
    typeof(Complaint).GetProperty("Complainant")!.SetValue(complaint, users.Single(u => u.Id == complaint.ComplainantUserId));
    complaints.Add(complaint); return Task.CompletedTask;
}
builder.Services.AddSingleton(complaintRepo);
builder.Services.AddSingleton(otpService);
// Provisioning mail fixture: captures the credentials the endpoint hands to the mail provider.
// The "mailfail" address simulates a provider outage so the manual hand-over path can be checked.
string? emailedCredentials = null;
string? emailedCredentialsTo = null;
var credentialsEmail = Fake.Create<ICredentialsEmailService>((method, args) => method switch {
    "SendTemporaryCredentialsAsync" => SendCredentials((UserAccount)args[0]!, (string)args[1]!),
    _ => throw new NotSupportedException(method)
});
Task SendCredentials(UserAccount user, string temporaryPassword) {
    if (user.Email.StartsWith("mailfail")) throw new InvalidOperationException("mail provider unavailable");
    emailedCredentialsTo = user.Email;
    emailedCredentials = temporaryPassword;
    return Task.CompletedTask;
}
builder.Services.AddSingleton(credentialsEmail);
builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<AdminLoginCommand>());
builder.Services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly)
    // Same contract as the real API: enums travel as their names.
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDigiFikileLmsSwagger();
builder.Services.AddDigiFikileLmsAuthentication(builder.Configuration);
await using var app = builder.Build();
app.UseSwagger();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
foreach (var role in Enum.GetValues<UserRole>())
    app.MapGet("/probe/" + role, () => Results.Ok()).RequireAuthorization(role.ToString());
await app.StartAsync();
using var client = new HttpClient { BaseAddress = new Uri(app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single()) };
int passed = 0;
void Check(bool ok, string name) { if (!ok) throw new Exception("FAIL: " + name); Console.WriteLine("PASS: " + name); passed++; }
async Task<(HttpStatusCode Status, AuthResponseDto? Data)> Post(string path, object body) {
    using var response = await client.PostAsJsonAsync("/api/Auth/" + path, body);
    var result = await response.Content.ReadFromJsonAsync<BaseResponse<AuthResponseDto>>();
    return (response.StatusCode, result?.Data);
}
// Full two-step login: password step, then the 6-digit OTP step that returns the JWT.
async Task<(HttpStatusCode Status, AuthResponseDto? Data)> Login(string path, object body, string email) {
    var challenge = await Post(path, body);
    if (challenge.Status != HttpStatusCode.OK) return challenge;
    return await Post("otp/verify", new { Email = email, Otp = OtpCode });
}
var registration = new { Name="Test", Surname="Learner", Email="learner@example.test", Password="Test-password-123!", StudentNumber="STU-TEST" };
Check((await Post("student/register", registration)).Status == HttpStatusCode.OK, "learner registration");
Check(users[0].UserRole == UserRole.Student && hasher.VerifyPassword(registration.Password, users[0].Password) && !hasher.NeedsRehash(users[0].Password), "registration assigns only Student and hashes password");
Check((await Post("student/register", registration)).Status == HttpStatusCode.BadRequest, "duplicate registration");
Check((await Post("student/register", registration with { Email="LEARNER@example.test", StudentNumber="DIFFERENT" })).Status == HttpStatusCode.BadRequest, "case-insensitive duplicate email");
Check((await Post("student/register", registration with { Email="other@example.test" })).Status == HttpStatusCode.BadRequest, "duplicate student number");
var learnerChallenge = await Post("student/login", new { registration.StudentNumber, registration.Password });
Check(learnerChallenge.Status == HttpStatusCode.OK && learnerChallenge.Data!.OtpRequired &&
    learnerChallenge.Data.Token.Length == 0 && learnerChallenge.Data.OtpExpiresInMinutes == 10 &&
    learnerChallenge.Data.OtpSentTo == "l*****r@example.test",
    "learner login returns a 6-digit OTP challenge instead of a JWT");
Check((await Post("otp/verify", new { Email = registration.Email, Otp = "000000" })).Status == HttpStatusCode.Unauthorized,
    "wrong 6-digit OTP is rejected");
Check((await Post("otp/verify", new { Email = registration.Email, Otp = "12345" })).Status == HttpStatusCode.Unauthorized,
    "OTP that is not six digits is rejected");
Check((await Post("otp/resend", new { registration.Email })).Status == HttpStatusCode.OK &&
    (await Post("otp/resend", new { Email = "missing@example.test" })).Status == HttpStatusCode.BadRequest,
    "OTP resend issues a new challenge for a known account only");
var learner = await Login("student/login", new { registration.StudentNumber, registration.Password }, registration.Email);
Check(learner.Status == HttpStatusCode.OK && tokens.ValidateToken(learner.Data!.Token), "learner login returns valid JWT after the OTP step");
Check((await Post("admin/login", new { registration.Email, registration.Password })).Status == HttpStatusCode.Unauthorized, "learner cannot use admin login");
Check((await Post("student/login", new { registration.StudentNumber, Password="wrong" })).Status == HttpStatusCode.Unauthorized, "invalid learner password returns 401");
var roleTokens = new Dictionary<UserRole,string> { [UserRole.Student] = learner.Data!.Token };
foreach (var role in new[] { UserRole.SystemAdministrator, UserRole.SetaAdministrator }) {
    var user = UserAccount.Create("Admin", "Test", role + "@example.test", hasher.HashPassword(registration.Password), role);
    await AddUser(user);
    var login = role == UserRole.SetaAdministrator
        ? await Login("admin/login", new { user.Email, registration.Password }, user.Email)
        : await Post("admin/login", new { user.Email, registration.Password });
    if (role == UserRole.SetaAdministrator)
        Check(login.Status == HttpStatusCode.OK && login.Data!.Role == role.ToString(), "SETA admin login succeeds after the OTP step");
    else
        Check(login.Status == HttpStatusCode.Unauthorized && login.Data == null, role + " cannot use SETA admin login");
    // Other-role tokens are fixtures for authorization checks, not issued by admin/login.
    roleTokens[role] = role == UserRole.SetaAdministrator ? login.Data!.Token : tokens.GenerateToken(user);
    Check((await Post("admin/login", new { user.Email, Password="wrong" })).Status == HttpStatusCode.Unauthorized, role + " invalid password");
}
// Staff logins: Facilitator, Moderator and Training Provider follow the same two-step handshake.
var staffLogins = new Dictionary<UserRole,(string Email, string Path)> {
    [UserRole.Facilitator] = ("facilitator@example.test", "facilitator/login"),
    [UserRole.Moderator] = ("moderator@example.test", "moderator/login"),
    [UserRole.TrainingProvider] = ("training-provider@example.test", "training-provider/login"),
};
foreach (var (role, staff) in staffLogins) {
    var staffUser = UserAccount.Create("Staff", role.ToString(), staff.Email, hasher.HashPassword(registration.Password), role);
    await AddUser(staffUser);
    var challenge = await Post(staff.Path, new { staffUser.Email, registration.Password });
    Check(challenge.Status == HttpStatusCode.OK && challenge.Data!.OtpRequired &&
        challenge.Data.Token == string.Empty && challenge.Data.OtpSentTo!.Contains('*'),
        role + " login issues an OTP challenge without a JWT");
    var staffToken = await Post("otp/verify", new { Email = staffUser.Email, Otp = OtpCode });
    Check(staffToken.Status == HttpStatusCode.OK && staffToken.Data!.Role == role.ToString() &&
        tokens.ValidateToken(staffToken.Data.Token), role + " receives its JWT after the OTP step");
    Check(new JwtSecurityTokenHandler().ReadJwtToken(staffToken.Data!.Token)
        .Claims.Single(c => c.Type == ClaimTypes.Role).Value == role.ToString(), role + " JWT carries only its own role");
    roleTokens[role] = staffToken.Data!.Token;
    Check((await Post(staff.Path, new { staffUser.Email, Password="wrong" })).Status == HttpStatusCode.Unauthorized, role + " invalid password");
    Check((await Post("otp/verify", new { Email = staffUser.Email, Otp = "654321" })).Status == HttpStatusCode.Unauthorized, role + " wrong PIN rejected");
}
Check((await Post("facilitator/login", new { Email=staffLogins[UserRole.Moderator].Email, registration.Password })).Status == HttpStatusCode.Unauthorized, "Moderator cannot use the facilitator login");
Check((await Post("moderator/login", new { Email=staffLogins[UserRole.Facilitator].Email, registration.Password })).Status == HttpStatusCode.Unauthorized, "Facilitator cannot use the moderator login");
Check((await Post("training-provider/login", new { Email=staffLogins[UserRole.Facilitator].Email, registration.Password })).Status == HttpStatusCode.Unauthorized, "Facilitator cannot use the training provider login");
Check((await Post("facilitator/login", new { Email=registration.Email, registration.Password })).Status == HttpStatusCode.Unauthorized, "Learner cannot use a staff login");
var jwt = new JwtSecurityTokenHandler().ReadJwtToken(learner.Data.Token);
Check(jwt.Subject == users[0].Id.ToString() && jwt.ValidTo > DateTime.UtcNow &&
    jwt.Claims.Any(c => c.Type == ClaimTypes.Email && c.Value == registration.Email) &&
    jwt.Claims.Any(c => c.Type == ClaimTypes.Name) &&
    jwt.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Student"), "JWT identity, email, name, role and expiry");
foreach (var target in roleTokens.Keys) {
    client.DefaultRequestHeaders.Authorization = null;
    Check((await client.GetAsync("/probe/" + target)).StatusCode == HttpStatusCode.Unauthorized, target + " missing token returns 401");
    foreach (var caller in roleTokens) {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", caller.Value);
        var expected = caller.Key == target ? HttpStatusCode.OK : HttpStatusCode.Forbidden;
        Check((await client.GetAsync("/probe/" + target)).StatusCode == expected, caller.Key + " -> " + target + " = " + (int)expected);
    }
}
foreach (var path in new[] { "/api/system-admin/profile", "/api/Users", "/api/SetaAdministrators", "/api/Roles", "/api/Permissions", "/api/complaints", "/api/complaints/1", "/api/Students" }) {
    client.DefaultRequestHeaders.Authorization = null;
    Check((await client.GetAsync(path)).StatusCode == HttpStatusCode.Unauthorized, path + " returns 401");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", learner.Data.Token);
    Check((await client.GetAsync(path)).StatusCode == HttpStatusCode.Forbidden, path + " rejects learner with 403");
}
foreach (var role in new[] { UserRole.SystemAdministrator, UserRole.SetaAdministrator }) {
    var user = users.Single(u => u.UserRole == role);
    if (role == UserRole.SystemAdministrator) {
        var profile = SystemAdministrator.Create(user); profile.Deactivate();
        typeof(UserAccount).GetProperty("SystemAdministrator")!.SetValue(user, profile);
    } else {
        var profile = SetaAdministrator.Create(user); profile.Deactivate();
        typeof(UserAccount).GetProperty("SetaAdministrator")!.SetValue(user, profile);
    }
    Check((await Post("admin/login", new { user.Email, registration.Password })).Status == HttpStatusCode.Unauthorized, role + " inactive login rejected");
}
users[0].UpdatePassword(Convert.ToBase64String(Encoding.UTF8.GetBytes(registration.Password)));
Check((await Post("student/login", new { registration.StudentNumber, registration.Password })).Status == HttpStatusCode.OK &&
    !hasher.NeedsRehash(users[0].Password), "legacy password upgraded on login");
// Complaints: any signed-in account can lay one; only the System Administrator sees every
// complaint and moves it through the workflow, and nobody can read or change another one.
async Task<(HttpStatusCode Status, ComplaintDto? Data)> PostComplaint(object body, string? token = null) {
    using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Complaints") { Content = JsonContent.Create(body) };
    if (token != null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    using var response = await client.SendAsync(request);
    // A 401 challenge has an empty body; only parse the envelope when the server wrote JSON.
    var length = response.Content.Headers.ContentLength;
    if (length.HasValue && length.Value == 0) return (response.StatusCode, null);
    var result = await response.Content.ReadFromJsonAsync<BaseResponse<ComplaintDto>>();
    return (response.StatusCode, result?.Data);
}
client.DefaultRequestHeaders.Authorization = null; // the restricted-endpoint loop above leaves the learner token behind
var anonLay = await PostComplaint(new { Title = "t", Description = "d" });
Check(anonLay.Status == HttpStatusCode.Unauthorized, "laying a complaint requires a token (got " + (int)anonLay.Status + ")");
Check((await PostComplaint(new { Title = "", Description = "missing title" }, learner.Data!.Token)).Status == HttpStatusCode.BadRequest, "complaint without a title is rejected");
Check((await PostComplaint(new { Title = new string('x', 201), Description = "too long" }, learner.Data!.Token)).Status == HttpStatusCode.BadRequest, "complaint title over 200 characters is rejected");
var learnerComplaint = await PostComplaint(new { Title = "Fees charged twice", Description = "The study fee was deducted twice this month.", Category = "Fees" }, learner.Data!.Token);
Check(learnerComplaint.Status == HttpStatusCode.OK && learnerComplaint.Data!.Status == nameof(ComplaintStatus.Open) &&
    learnerComplaint.Data.ComplainantName == $"{users[0].Name} {users[0].Surname}" && learnerComplaint.Data.ComplainantEmail == registration.Email,
    "learner lays a complaint and it starts Open");
var systemUser = users.Single(u => u.UserRole == UserRole.SystemAdministrator);
var systemComplaint = await PostComplaint(new { Title = "Laptop stolen", Description = "Reporting a stolen laptop." }, roleTokens[UserRole.SystemAdministrator]);
Check(systemComplaint.Status == HttpStatusCode.OK && systemComplaint.Data!.Status == nameof(ComplaintStatus.Open), "System Administrator lays a complaint too");
var staffLoginTokens = staffLogins.ToDictionary(p => p.Key, p => tokens.GenerateToken(users.Single(u => u.Email == p.Value.Email)));
// A JWT is only minted at otp/verify in production, so any valid token is a completed session;
// a token minted directly here is indistinguishable and must be accepted.
Check((await PostComplaint(new { Title = "Project files withheld", Description = "Facilitator complaint." }, staffLoginTokens[UserRole.Facilitator])).Status == HttpStatusCode.OK, "Facilitator lays a complaint");
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", staffLoginTokens[UserRole.Moderator]);
Check((await client.GetAsync("/api/Complaints/1")).StatusCode == HttpStatusCode.Forbidden, "Moderator cannot read the learner complaint");
var mine = await client.GetFromJsonAsync<BaseResponse<List<ComplaintDto>>>("/api/Complaints/mine");
Check(mine!.IsSuccess && mine.Data!.Count == 0, "Moderator sees none of the learner complaints as their own");
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", learner.Data!.Token);
HttpResponseMessage dbg = await client.GetAsync("/api/Complaints/mine");
if (!dbg.IsSuccessStatusCode) throw new Exception("/mine failed: " + dbg.StatusCode + " " + await dbg.Content.ReadAsStringAsync());
var learnerList = await dbg.Content.ReadFromJsonAsync<BaseResponse<List<ComplaintDto>>>();
Check(learnerList!.IsSuccess && learnerList.Data!.Single(c => c.Id == 1).Title == "Fees charged twice", "learner sees their own complaint");
Check((await client.GetAsync("/api/Complaints")).StatusCode == HttpStatusCode.Forbidden, "learner cannot list every complaint");
Check((await client.GetAsync("/api/Complaints/2")).StatusCode == HttpStatusCode.Forbidden, "learner cannot read the System Administrator complaint");
Check((await client.PutAsJsonAsync("/api/Complaints/1/status", new { Status = "Resolved", ResolutionNotes = "self-approved" })).StatusCode == HttpStatusCode.Forbidden, "learner cannot update a complaint status");
Check((await client.GetAsync("/api/Complaints/999")).StatusCode == HttpStatusCode.Forbidden, "learner cannot probe an unknown complaint");
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", roleTokens[UserRole.SystemAdministrator]);
Check((await client.GetAsync("/api/Complaints/999")).StatusCode == HttpStatusCode.NotFound, "unknown complaint returns 404 for the System Administrator");
HttpResponseMessage invalidStatus = await client.PutAsJsonAsync("/api/Complaints/1/status", new { Status = "NotAStatus" });
Check(invalidStatus.StatusCode == HttpStatusCode.BadRequest,
    "invalid status name is rejected (got " + (int)invalidStatus.StatusCode + " " + await invalidStatus.Content.ReadAsStringAsync() + ")");
if (!invalidStatus.IsSuccessStatusCode) invalidStatus.Dispose();
var filtered = await client.GetFromJsonAsync<BaseResponse<List<ComplaintDto>>>("/api/Complaints?status=Open");
Check(filtered!.IsSuccess && filtered.Data!.Count == 3 && filtered.Data!.All(c => c.Status == nameof(ComplaintStatus.Open)), "System Administrator lists every complaint filtered by status");
var resolved = await client.PutAsJsonAsync("/api/Complaints/1/status", new { Status = nameof(ComplaintStatus.Resolved), ResolutionNotes = "Duplicate fee refunded." });
var resolvedDto = await resolved.Content.ReadFromJsonAsync<BaseResponse<ComplaintDto>>();
Check(resolved.StatusCode == HttpStatusCode.OK && resolvedDto!.IsSuccess && resolvedDto.Data!.Status == nameof(ComplaintStatus.Resolved) &&
    resolvedDto.Data.ResolutionNotes == "Duplicate fee refunded." && resolvedDto.Data.ResolvedAt != null,
    "System Administrator resolves the learner complaint");
var myOpen = await client.GetFromJsonAsync<BaseResponse<List<ComplaintDto>>>("/api/Complaints?status=Open");
Check(myOpen!.Data!.Count == 2 && myOpen.Data!.All(c => c.Id != 1), "resolved complaint leaves the Open filter");
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", learner.Data!.Token);
var learnerView = await client.GetFromJsonAsync<BaseResponse<List<ComplaintDto>>>("/api/Complaints/mine");
Check(learnerView!.Data!.Single(c => c.Id == 1).Status == nameof(ComplaintStatus.Resolved), "learner sees the resolution on their complaint");
client.DefaultRequestHeaders.Authorization = null;
var adminRepo = Fake.Create<IAdministratorRepository>((method, args) => method switch {
    "AddAsync" => AttachSeta((SetaAdministrator)args[0]!),
    "SaveChangesAsync" => Task.FromResult(1),
    _ => throw new NotSupportedException(method)
});
Task AttachSeta(SetaAdministrator profile) {
    typeof(UserAccount).GetProperty("SetaAdministrator")!.SetValue(profile.User, profile);
    return Task.CompletedTask;
}
var auditLogs = new List<SystemLog>();
var logRepo = Fake.Create<ISystemLogRepository>((method, args) => method switch {
    "AddAsync" => VerifyAudit((SystemLog)args[0]!), "SaveChangesAsync" => Task.FromResult(1),
    _ => throw new NotSupportedException(method)
});
Task VerifyAudit(SystemLog log) {
    Check(log.SystemAdministratorId == 42, "SETA creation audit uses authenticated System Admin profile ID");
    auditLogs.Add(log);
    return Task.CompletedTask;
}
var initiator = SystemAdministrator.Create(users.Single(u => u.UserRole == UserRole.SystemAdministrator));
typeof(SystemAdministrator).GetProperty("Id")!.SetValue(initiator, 42);
var systemRepo = Fake.Create<ISystemAdministratorRepository>((method, args) => method == "GetByUserIdAsync"
    ? Task.FromResult<SystemAdministrator?>(initiator) : throw new NotSupportedException(method));
var create = new DigiFikileLms.Application.Features.SystemAdmin.CreateSetaAdministratorCommandHandler(userRepo, adminRepo, hasher, logRepo, systemRepo);
var created = await create.Handle(new("Created","Seta","created@example.test",registration.Password,null,null,new(),initiator.UserId), default);
Check(created.IsSuccess && (await Post("admin/login", new { Email="created@example.test", registration.Password })).Status == HttpStatusCode.OK, "System Admin creation flow produces SETA account that can log in");
// New endpoint: the System Admin registers a SETA administrator whose username and temporary
// password are e-mailed to the account holder; the response reports the mail outcome and only
// carries the password when the mail could not be sent.
var tempPasswords = new TemporaryPasswordGenerator();
var provision = new DigiFikileLms.Application.Features.SystemAdmin.ProvisionSetaAdminCommandHandler(
    userRepo, adminRepo, hasher, tempPasswords, credentialsEmail, logRepo, systemRepo);
var credentials = await provision.Handle(new("Provisioned","Seta","provisioned@example.test",null,null,new(),initiator.UserId), default);
var provisioned = users.Single(u => u.Email == "provisioned@example.test");
Check(credentials.IsSuccess && credentials.Data!.Username == "provisioned@example.test" &&
    credentials.Data.CredentialsEmailed && credentials.Data.TemporaryPassword == null &&
    credentials.Data.EmailStatus.Contains("provisioned@example.test") && credentials.Data.PasswordIsTemporary,
    "provision returns the username and reports the credentials were e-mailed");
Check(emailedCredentialsTo == "provisioned@example.test" && emailedCredentials!.Length >= 8 &&
    hasher.VerifyPassword(emailedCredentials, provisioned.Password),
    "credentials e-mail goes only to the account holder and carries the working password");
Check(provisioned.UserRole == UserRole.SetaAdministrator && provisioned.IsActive,
    "provisioned account is an active SETA administrator");
var provisionedLogin = await Login("admin/login", new { Email="provisioned@example.test", Password=emailedCredentials }, "provisioned@example.test");
Check(provisionedLogin.Status == HttpStatusCode.OK && tokens.ValidateToken(provisionedLogin.Data!.Token),
    "provisioned SETA administrator signs in with the e-mailed password and the OTP");
using (var change = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/password/change") {
    Content = JsonContent.Create(new { CurrentPassword = emailedCredentials, NewPassword = "Changed-Password-123!" }) }) {
    change.Headers.Authorization = new AuthenticationHeaderValue("Bearer", provisionedLogin.Data!.Token);
    Check((await client.SendAsync(change)).StatusCode == HttpStatusCode.OK, "temporary password can be replaced after the first login");
}
Check((await Login("admin/login", new { Email="provisioned@example.test", Password="Changed-Password-123!" }, "provisioned@example.test")).Status == HttpStatusCode.OK,
    "SETA administrator signs in with the replaced password");
Check((await Post("admin/login", new { Email="provisioned@example.test", Password=emailedCredentials })).Status == HttpStatusCode.Unauthorized,
    "temporary password stops working after the change");
// Mail provider outage: the account stays usable and the password is returned for hand-over.
var outage = await provision.Handle(new("Outage","Seta","mailfail@example.test",null,null,new(),initiator.UserId), default);
var outageUser = users.Single(u => u.Email == "mailfail@example.test");
Check(outage.IsSuccess && !outage.Data!.CredentialsEmailed && outage.Data.TemporaryPassword!.Length >= 8 &&
    hasher.VerifyPassword(outage.Data.TemporaryPassword, outageUser.Password) &&
    outage.Data.EmailStatus.Contains("manually"),
    "mail outage keeps the account and returns the password for manual hand-over");
Check((await Login("admin/login", new { Email="mailfail@example.test", Password=outage.Data!.TemporaryPassword }, "mailfail@example.test")).Status == HttpStatusCode.OK,
    "manually handed-over password signs in after the mail outage");
Check(auditLogs.Any(l => l.Description!.Contains("credentials e-mailed: True")) &&
    auditLogs.Any(l => l.Description!.Contains("credentials e-mailed: False")),
    "audit records the provisioning and the mail outcome without the password");
client.DefaultRequestHeaders.Authorization = null;
using (var unauth = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/password/change") {
    Content = JsonContent.Create(new { CurrentPassword="Changed-Password-123!", NewPassword="Another-Password-123!" }) }) {
    Check((await client.SendAsync(unauth)).StatusCode == HttpStatusCode.Unauthorized, "password change requires an authenticated caller");
}
Check((await Post("system-admin/login", new { Email="provisioned@example.test", Password="Changed-Password-123!" })).Status == HttpStatusCode.Unauthorized,
    "SETA administrator cannot use the System Admin login");
Check(!tokens.ValidateToken("placeholder-jwt-token-for-development") && !tokens.ValidateToken(learner.Data.Token + "broken"), "malformed and tampered tokens rejected");
var expired = new JwtSecurityToken("AuthChecks","AuthChecks", expires:DateTime.UtcNow.AddMinutes(-5),
    signingCredentials:new SigningCredentials(AuthenticationExtensions.CreateTokenValidationParameters(builder.Configuration).IssuerSigningKey,SecurityAlgorithms.HmacSha256));
Check(!tokens.ValidateToken(new JwtSecurityTokenHandler().WriteToken(expired)), "expired JWT rejected");
foreach (var role in new[] { UserRole.Student, UserRole.SetaAdministrator })
{
    // Simulate persisted accounts from before JWT activation, without registering/recreating them.
    var existing = UserAccount.Create("Existing", "Account", $"existing-{role}@example.test",
        Convert.ToBase64String(Encoding.UTF8.GetBytes(registration.Password)), role);
    await AddUser(existing);
    if (role == UserRole.Student) await AddStudent(Student.Create(existing, "EXISTING-STUDENT"));
    else await AttachSeta(SetaAdministrator.Create(existing));
    var originalId = existing.Id;
    var originalCount = users.Count;
    var login = role == UserRole.Student
        ? await Login("student/login", new { StudentNumber="EXISTING-STUDENT", registration.Password }, existing.Email)
        : await Login("admin/login", new { existing.Email, registration.Password }, existing.Email);
    Check(login.Status == HttpStatusCode.OK && login.Data!.UserId == originalId &&
        users.Count == originalCount && tokens.ValidateToken(login.Data.Token),
        $"existing {role} logs in without recreation");
    var roleClaims = new JwtSecurityTokenHandler().ReadJwtToken(login.Data!.Token)
        .Claims.Where(c => c.Type == ClaimTypes.Role).ToArray();
    Check(roleClaims.Length == 1 && roleClaims[0].Value == role.ToString(),
        $"{role} JWT contains exactly its own role");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Data.Token);
    Check((await client.GetAsync("/api/Users"))
        .StatusCode == HttpStatusCode.Forbidden, $"{role} rejected from restricted real controller");
}
var validation = AuthenticationExtensions.CreateTokenValidationParameters(builder.Configuration);
string SignedToken(string issuer, string audience, SecurityKey key, DateTime expiry) =>
    new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(issuer, audience,
        new[] { new Claim(ClaimTypes.Role, "Student") }, expires: expiry,
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)));
var invalidTokens = new Dictionary<string,string> {
    ["malformed"] = "invalid-token",
    ["expired"] = new JwtSecurityTokenHandler().WriteToken(expired),
    ["wrong issuer"] = SignedToken("wrong", "AuthChecks", validation.IssuerSigningKey, DateTime.UtcNow.AddMinutes(5)),
    ["wrong audience"] = SignedToken("AuthChecks", "wrong", validation.IssuerSigningKey, DateTime.UtcNow.AddMinutes(5)),
    ["wrong signing key"] = SignedToken("AuthChecks", "AuthChecks",
        new SymmetricSecurityKey(System.Security.Cryptography.RandomNumberGenerator.GetBytes(48)), DateTime.UtcNow.AddMinutes(5))
};
foreach (var invalid in invalidTokens)
{
    Check(!tokens.ValidateToken(invalid.Value), invalid.Key + " fails token validation");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalid.Value);
    Check((await client.GetAsync("/probe/Student")).StatusCode == HttpStatusCode.Unauthorized,
        invalid.Key + " returns HTTP 401");
}
client.DefaultRequestHeaders.Authorization = null;
var swagger = await client.GetStringAsync("/swagger/v1/swagger.json");
Check(swagger.Contains("\"Bearer\"") && swagger.Contains("/api/Auth/student/register") && swagger.Contains("/api/Auth/admin/login") &&
    swagger.Contains("/api/Auth/otp/verify") && swagger.Contains("/api/Auth/otp/resend") && swagger.Contains("/api/Auth/password/change") &&
    swagger.Contains("/api/Auth/system-admin/login") && swagger.Contains("/api/system-admin/seta-admin/provision") &&
    swagger.Contains("/api/Auth/facilitator/login") && swagger.Contains("/api/Auth/moderator/login") &&
    swagger.Contains("/api/Auth/training-provider/login") && swagger.Contains("/api/Complaints"),
    "Swagger Bearer, two-step login, staff logins, provisioning and complaint routes");
Check(hasher.HashPassword(registration.Password) != hasher.HashPassword(registration.Password), "password salts are random");
await app.StopAsync();
Console.WriteLine($"All {passed} authentication checks passed.");

public class Fake : DispatchProxy {
    public Func<string,object?[],object?> InvokeMethod { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => InvokeMethod(method!.Name, args!);
    public static T Create<T>(Func<string,object?[],object?> handler) where T:class {
        var proxy = Create<T,Fake>(); ((Fake)(object)proxy).InvokeMethod=handler; return proxy;
    }
}