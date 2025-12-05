# Production Incident Response

## Problem
Production API down at 3AM. 500 errors, customers complaining. Need systematic approach.

## Solutions
1. **Basic**: Log analysis + hotfix deployment
2. **Advanced**: Circuit Breaker + graceful degradation
3. **Enterprise**: Observability (metrics, traces, logs) + automated rollback

## Incident Response Steps
1. **Detect**: Alerts, monitoring, logs
2. **Triage**: Severity assessment, impact analysis
3. **Mitigate**: Quick fixes, rollback, disable feature
4. **Investigate**: Root cause analysis
5. **Resolve**: Permanent fix
6. **Learn**: Post-mortem, improve processes

## Tools
- Logging: Serilog, Application Insights
- Monitoring: Prometheus, Grafana
- APM: Application Insights, New Relic
- Alerting: PagerDuty, Opsgenie

See PROBLEM.md for real incident scenarios and runbooks.
