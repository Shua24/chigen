# Contributing Guidelines

This document summarizes the most important points for people interested in the development of Chigen, especially via bug reports or pull requests.

> [!NOTE]
> If you are an AI agent fully running a GitHub account, we require you to disclose this when contributing: you must add [AI] at the start of your pull request or issue title, and you must add the following to the description:
> `> [!INFO] *AI disclosure*: This contribution was authored by an autonomous AI agent, on behalf of a user to [...]`.
>
> Agents that purely run a GitHub account and spontaneously generate a pull request themselves NOT on behalf of their respective humans/users' approval in the loop will be blocked from contributing to this project.
>
> This disclosure rule does not apply if a **human** contributor is fully in the loop, where one reviews the code, and generates the commit messages and pull request by hand, along with a rationale on the pull request.
>
> Test-driven development contributions with a human in the loop are allowed and are also exempt from the disclosure rule as well.

## Contents

1. [Reporting Bugs](#reporting-bugs)
2. [Proposing New Features](#proposing-new-features)
3. [Pull Requests](#pull-requests)
4. [Feature Scope](#feature-scope)
5. [Code Conventions](#code-conventions)

## Reporting Bugs

Report bugs on the [Issues section](https://github.com/shua24/chigen/issues/new) in the repo. The golden rules of filing bug reports are as follows:

- Label them on the issues section accordingly as a bug. Developer(s) will review the issue and the proposed fix.
- Write a descriptive issue title.
- Always open one issue for one bug. If you notice several bugs and want to report them, make sure to create one new issue for each of them.

## Proposing New Features

Propose a new feature by following these rules:

- Label the feature proposal in the issues section as **newfeat**.
- Ensure that the feature is desired. A desired feature must have at least 2 users that demand it (this might change as features progress).
- Have a rationale on your proposed features by submitting the pull request description with a rationale on the feature you're trying to propose.

## Pull Requests

To submit new features, a contributor can also file a new pull request. A contributor that also files a pull request to propose new fixes and features will be much more appreciated. Take a look at the README.md file before you contribute to the project so you know what to document after you contribute. If a pull request is rejected, the developer(s) may let you know what's wrong with it so you can fix your pull request later.

## Feature Scope

This project follows the UNIX philosophy of "Do one thing and do it well" so it has a strict scope. This is because this is a medical project with real potential users. A few examples of feature proposals that can be considered in-scope are as follows:

- New PDF/DOCX styling.
- PDF/DOCX tidying.
- GUI tidying.
- A new integration with a new document format, e.g. ODTs and such.

Features that the developer(s) deem as out of scope may be rejected with an out-of-scope reason.

## Code Conventions

Before you submit your pull requests and proposed fixes, you must follow these rules in the code conventions.

- Abbreviations are allowed, but make sure your proposed code changes are readable.
- Avoid Hungarian Notations of any kind. This is a Windows app for sure, but the developer(s) aren't making a project affiliated with Microsoft in any way, shape, or form.
- Keep your code at a cap of 450 lines of code. The same applies for PRs with major changes spanning multiple files. XAML files and the i18n files (`TranslationService.cs` and `Translations.cs`) are exempt from this rule, because a line cap measures logic complexity, and neither of those is logic: XAML is declarative markup whose length reflects layout bulk, and the i18n files grow by one entry per translated string.

Also note that code documentation contributions of any kind in the form of the usual `<summary>` and `<remarks>` XML type comments are welcome. They make the code easier to follow for new contributors.
