using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class CertificateRepository : ICertificateRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public CertificateRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Certificate?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Certificates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default)
        => await _context.Certificates.AsNoTracking().FirstOrDefaultAsync(x => x.CertificateNumber == certificateNumber, cancellationToken);

    public async Task<IEnumerable<Certificate>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default)
        => await _context.Certificates.AsNoTracking().Where(x => x.Result.StudentId == studentId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Certificate>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default)
        => await _context.Certificates.AsNoTracking().Where(x => x.ResultId == courseId).ToListAsync(cancellationToken);

    public async Task<bool> VerifyCertificateAsync(string certificateNumber, CancellationToken cancellationToken = default)
        => await _context.Certificates.AsNoTracking().AnyAsync(x => x.CertificateNumber == certificateNumber, cancellationToken);

    public async Task<IEnumerable<Certificate>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Certificates.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        await _context.Certificates.AddAsync(certificate, cancellationToken);
    }

    public Task UpdateAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        _context.Certificates.Update(certificate);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Certificates.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Certificates.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
