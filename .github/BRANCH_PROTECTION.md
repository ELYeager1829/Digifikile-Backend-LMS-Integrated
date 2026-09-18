# Branch Protection Setup

The `branch-rules.yml` workflow enforces merge rules on pull requests. To fully protect your branches, you must also configure **branch protection rules** in GitHub (these cannot be set via workflow files).

## Required GitHub Settings

Go to **Settings → Branches → Add branch protection rule** (or edit existing) for each of: `main`, `master`, `development`, `dev`.

### For `main` and `master`

1. **Branch name pattern:** `main` (create a separate rule for `master` if you use both)
2. **Require a pull request before merging:** ✓
   - Required approvals: 1 (or more, as you prefer)
3. **Require status checks to pass before merging:** ✓
   - Add: `Branch Rules`, `build` (or `CI` / `build` from your CI workflow)
4. **Do not allow bypassing the above settings:** ✓ (recommended)
5. **Restrict who can push to matching branches:** Leave empty so only PRs can update main
6. **Allow specified actors to bypass required pull requests:** Add `Lethabo-T-Molefe` so they can push directly and bypass all rules

### For `development` and `dev`

1. **Branch name pattern:** `development` or `dev`
2. **Require a pull request before merging:** ✓
3. **Require status checks to pass before merging:** ✓
   - Add: `Branch Rules`, `build` (or your CI job name)
4. **Do not allow bypassing the above settings:** Uncheck this if you added Lethabo-T-Molefe as a bypass actor (otherwise they cannot bypass)
5. **Allow specified actors to bypass required pull requests:** Add `Lethabo-T-Molefe` so they can push directly and bypass all rules

## Merge Rules (enforced by `branch-rules.yml`)

**Bypass:** The user `Lethabo-T-Molefe` is exempt from merge rules in the workflow and can open/merge any PR.

| Target branch | Allowed source branches |
|---------------|-------------------------|
| `main` / `master` | Only `dev` or `development` |
| `dev` / `development` | Any branch except `main` / `master` |

## Flow

```
feature/xyz  ──PR──►  dev/development  ──PR──►  main
     ✓                      ✓
```

- Work on feature branches → open PR into `dev` or `development`
- When ready for release → open PR from `dev` or `development` into `main`
