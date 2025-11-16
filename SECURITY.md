# Security Policy

## Supported Versions

We release patches for security vulnerabilities. Which versions are eligible for
receiving such patches depends on the CVSS v3.0 Rating:

| Version | Supported          |
| ------- | ------------------ |
| 1.x.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take the security of our software seriously. If you believe you have found a security vulnerability in this project, we encourage you to let us know right away. We will investigate all legitimate reports and do our best to quickly fix the problem.

### How to Report a Security Vulnerability?

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, please report them via email to [dogaaydinn@gmail.com](mailto:dogaaydinn@gmail.com).

You should receive a response within 48 hours. If for some reason you do not, please follow up via email to ensure we received your original message.

Please include the following information in your report:

* Type of issue (e.g., buffer overflow, SQL injection, cross-site scripting, etc.)
* Full paths of source file(s) related to the manifestation of the issue
* The location of the affected source code (tag/branch/commit or direct URL)
* Any special configuration required to reproduce the issue
* Step-by-step instructions to reproduce the issue
* Proof-of-concept or exploit code (if possible)
* Impact of the issue, including how an attacker might exploit the issue

This information will help us triage your report more quickly.

### Preferred Languages

We prefer all communications to be in English.

### Security Update Policy

When we learn of a security vulnerability, we will:

1. **Confirm the Problem**: Verify the vulnerability and determine its severity
2. **Develop a Fix**: Create a patch that addresses the vulnerability
3. **Test the Fix**: Ensure the fix resolves the vulnerability without introducing new issues
4. **Release the Fix**: Deploy the fix in a new release
5. **Announce the Fix**: Publish a security advisory detailing the vulnerability and the fix

### Disclosure Policy

* Security issues are handled privately until a fix is available
* We will coordinate with you to determine an appropriate disclosure timeline
* We aim to fix critical vulnerabilities within 14 days
* We will publicly disclose the vulnerability after a fix has been released

### Security Best Practices for Users

To ensure the security of your implementation:

1. **Keep Dependencies Updated**: Regularly update to the latest version
2. **Monitor Security Advisories**: Subscribe to our GitHub repository to receive notifications
3. **Review Code**: Perform security reviews of any custom implementations
4. **Report Issues**: If you discover a vulnerability, report it immediately

### Security Hall of Fame

We recognize and thank security researchers who help keep our project secure:

* (No security reports yet)

### Comments on this Policy

If you have suggestions on how this process could be improved, please submit a pull request.

### Additional Resources

* [OWASP Top 10](https://owasp.org/www-project-top-ten/)
* [CWE Top 25](https://cwe.mitre.org/top25/)
* [.NET Security Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/security/)

---

**Last Updated**: 2025-01-14
