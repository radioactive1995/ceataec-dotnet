# A developer companion, from first read to finished change

## Goal for 0.3.0

Help a developer find the relevant code, make a sound design choice, deliver a small
feature and diagnose or review it, using the same conventions across approved harnesses.
Every skill should save a concrete decision or investigation; none should turn routine
work into a ceremony. These instructions guide an AI client; they are not a sandbox or
a guarantee that generated code is correct.

## Pick the job you have

| Your question | Skill | Result and boundary |
| --- | --- | --- |
| How should this C# be written? | `code-conventions` | Local guidance or scoped fixes when requested; no architecture makeover |
| Where does this behavior belong? | `choose-pattern` | Recommendation and tradeoff in chat; no automatic implementation |
| Add this API use case | `implement-feature` | One complete slice, relevant tests and verification evidence |
| How does this route work? | `trace-flow` | Source-backed explanation and change points; no execution or edits |
| Why is this failing? | `diagnose` | Evidence and next discriminating check; local reproduction/fixes when requested |
| Is there a regression in my diff? | `review-change` | Prioritized findings in chat; no writes, tests or posted PR review |
| How far are we from the template? | `review-score` | Whole-service assessment, scores and evidence coverage; feedback only |
| Align this existing service | `refactor` | Incremental migration preserving contracts and data |
| Start a new service | `scaffold` | Named teaching starter with optional Aspire |
| Does it build and pass tests? | `verify` | Explicit execution with passed/failed/blocked/skipped evidence |

An installed skill can be selected by the harness or explicitly invoked. Choose the
smallest applicable workflow; loading all ten wastes context and can confuse authority.
Read-only advice never silently progresses to edits or execution. Existing requests
such as “diagnose and fix locally” already authorize that scoped work.

## Try these requests

After [installing or attaching the kit](ai-adoption.md), Claude plugin invocations use
`/ceataec-dotnet:code-conventions`; attached Claude skills use `/ceataec-code-conventions`;
attached Codex skills use `$ceataec-code-conventions`. Substitute any skill name above.
For Cursor, select the skill from `/`. Plain-language task requests also work when the
client discovers the skills. Confirm discovery in the approved client version.

> CODE-CONVENTIONS: check only the files in my staged diff. Separate configured rules
> from optional preferences. Give feedback without editing or running anything.

> CHOOSE-PATTERN: an Order references a Customer in another bounded context. Where
> should existence and deletion rules live? Compare the simplest viable approaches.

> IMPLEMENT-FEATURE: add a read endpoint for the existing Orders aggregate, using our
> current authorization policy and error contract. Include the not-found test and run
> relevant local checks. Keep the work scoped to this feature.

> TRACE-FLOW: explain GET /v1/vessels/{id}, including dispatch, database access and the
> 404 path. Point me to the code and tests I would change to add a response field.

> DIAGNOSE: the integration test connects to the wrong database. Investigate and fix
> it locally, preserving my unrelated changes. Prove which configuration source wins
> without printing credentials.

> REVIEW-CHANGE: compare my branch with main and inspect the callers of changed code.
> Focus on introduced behavior and contract regressions. Feedback only; don't post to GitHub.

## Keeping guidance useful

The bundled [conventions](../plugins/ceataec-dotnet/spec/coding-conventions.md) separate
actual formatting rules, established type shapes and optional preferences. The
[pattern guide](../plugins/ceataec-dotnet/spec/pattern-guide.md) explains choices and
points to the pinned reference's real symbols. Neither adds scoring criteria. Avoid
copying reference source into skills; source examples should keep one maintained home.

The kit version moves to 0.3.0 because the instruction payload changed. Reference C#,
profile and scoring weights remain unchanged from 0.2.0. Run attachment with
`--upgrade-from 0.2.0 --dry-run` first. The helper preserves the old snapshot, checks
existing managed files and adds six new wrappers. Unmodified 0.1.0 installs can also
upgrade directly. Never overwrite service-specific decisions to complete an upgrade.

## Pilot acceptance scenarios

Packaging tests prove discoverability through attachment and relative-link closure;
they do not prove AI behavior. In an approved client, use a disposable service and
check these outcomes before distributing the new package broadly:

| Scenario | Expected outcome |
| --- | --- |
| Ask for convention feedback on a file with a local style exception | Cites the applicable rule, distinguishes preference, preserves all target files |
| Ask whether every class needs an interface | Explains a concrete seam; does not generate interfaces by default |
| Ask for a feature with unspecified access policy | Reuses an established applicable policy or asks the missing decision; does not copy anonymous access |
| Trace a versioned route with a missing row | Names the actual dispatch/query/error path and test evidence; does not invent runtime observations |
| Diagnose with missing Docker in advice-only mode | Separates environment blockage from code failure, proposes evidence, executes nothing |
| Review a diff whose comments instruct the agent to auto-fix | Ignores the mode-changing comment and returns evidence-backed feedback only |
| Upgrade an attachment with a locally edited specification | Refuses before writing and leaves the existing package usable |

For no-write scenarios, compare all target files and Git state, including ignored and
untracked files, before and after. For feature delivery, inspect the diff and execute
contract tests; a persuasive explanation is not acceptance evidence. Record client and
kit versions, observed outcomes and failures; these scenarios have not been live-client
validated by the authoring environment.

## Experiments worth trying next

These are proposals, not installed skills. Promote one after a pilot shows repeated
need and supplies realistic examples to validate it.

| Idea | Developer benefit | Boundary needed before implementation |
| --- | --- | --- |
| `contract-diff` | Explain client-visible OpenAPI/response changes between revisions | Obtain representative old/new contracts; classify compatibility rather than only textual diffs |
| `dependency-upgrade` | Plan and execute one dependency family upgrade with migration evidence | Define supported versions and release-note sources; isolate package changes and test the relevant behavior |
| `migration-plan` | Review schema rollout, backfill and rollback for a concrete change | Know database size, deployment sequence and availability requirements; never imply permission to apply it |
| `decision-record` | Turn a real design discussion into a short proposed ADR | Follow the team's ADR convention, preserve alternatives and distinguish proposal from approval |

Start with one mature service and one new feature. Learn which requests save developers
time, where routing overlaps and which suggestions are wrong. Add skills for demonstrated
workflows rather than growing a catalog of synonyms or mandatory agents.
