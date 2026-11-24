# Contributing

Try the browser lessons at [pritishchoudhary.com](https://www.pritishchoudhary.com/). Public visibility does not grant write access to this repository. External contributors should fork it and open a pull request.

`main` requires a pull request and an approving review from its code owner, @pritchoudhary. New reviewable commits dismiss stale approvals; all review conversations must be resolved. Direct pushes, force pushes and deletion of `main` are blocked. No bot or collaborator has a bypass.

Only @pritchoudhary has a pull-request-only review bypass, so he can personally merge a PR authored by his own account: GitHub does not allow self-approval. This exception never permits direct pushes, force pushes or branch deletion. Automated agents sharing his credentials must leave merges to him unless he explicitly approves that specific merge. GitHub cannot distinguish an agent from the account owner.

Use a feature branch and include the purpose, behavior change and actual validation in the PR. Run checks appropriate to the change; implementation changes normally require `npm test`, `npm run build` and `npm run typecheck`. Keep native Unity, browser and exported-player evidence distinct.

Do not weaken protection settings or publish generated website artifacts to bypass review. Repository rules are managed in GitHub Settings; documentation is not itself enforcement.
