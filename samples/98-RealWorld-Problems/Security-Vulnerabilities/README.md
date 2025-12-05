# Security Vulnerabilities

## Problem
Application has SQL injection, XSS, insecure authentication. Need to fix OWASP Top 10 vulnerabilities.

## Common Vulnerabilities
1. **SQL Injection**: Unsanitized user input in queries
2. **XSS (Cross-Site Scripting)**: Unescaped HTML output
3. **CSRF (Cross-Site Request Forgery)**: No anti-forgery tokens
4. **Insecure Authentication**: Weak passwords, no MFA
5. **Sensitive Data Exposure**: Passwords in plain text
6. **Security Misconfiguration**: Debug mode in production
7. **XXE (XML External Entity)**: Unsafe XML parsing
8. **Insecure Deserialization**: Arbitrary code execution
9. **Insufficient Logging**: No audit trail
10. **SSRF (Server-Side Request Forgery)**: Unvalidated URLs

## Solutions
1. **Basic**: Input validation + parameterized queries
2. **Advanced**: Authentication (JWT) + Authorization (policies)
3. **Enterprise**: Security scanning + penetration testing + compliance

## Best Practices
- Use parameterized queries (never string concatenation)
- Encode HTML output (prevent XSS)
- Use anti-forgery tokens
- Implement proper authentication (ASP.NET Identity, OAuth)
- Hash passwords (bcrypt, Argon2)
- Enable HTTPS only
- Security headers (CSP, HSTS, X-Frame-Options)
- Regular security audits

See PROBLEM.md for vulnerable code examples and fixes.
