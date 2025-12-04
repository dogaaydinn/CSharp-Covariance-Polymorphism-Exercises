# Junior Developer Growth Plan Template

**Mentor:** [Your Name]  
**Mentee:** [Junior Developer Name]  
**Start Date:** [Date]  
**Review Frequency:** Bi-weekly (every 2 weeks)  
**Goal:** Transition from Junior → Mid-Level Engineer in 12 months

---

## Current Level Assessment (Month 0)

### Technical Skills (1-5 scale)

| Skill | Rating | Evidence |
|-------|--------|----------|
| **OOP Fundamentals** | 2/5 | Can write classes, but uses if/else for type checking |
| **SOLID Principles** | 1/5 | Not familiar with concepts yet |
| **Design Patterns** | 1/5 | No experience |
| **Database/EF Core** | 3/5 | Can write queries, but has N+1 issues |
| **REST API Design** | 2/5 | APIs work, but inconsistent status codes |
| **Testing** | 2/5 | Writes manual tests, few automated tests |
| **Async/Await** | 2/5 | Uses it but doesn't understand Task lifecycle |
| **Git/Version Control** | 3/5 | Basic commits/pushes, struggles with merge conflicts |
| **Code Review** | 1/5 | Doesn't review others' code yet |

### Soft Skills

| Skill | Rating | Evidence |
|-------|--------|----------|
| **Communication** | 3/5 | Asks questions, sometimes unclear |
| **Problem Solving** | 3/5 | Can solve problems with guidance |
| **Initiative** | 4/5 | Eager to learn, volunteers for tasks |
| **Time Management** | 2/5 | Struggles with estimates |
| **Accepting Feedback** | 4/5 | Takes feedback well, implements suggestions |

### Strengths ✨
- Eager to learn
- Asks good questions
- Solid foundation in C# syntax
- Good work ethic

### Growth Areas 🎯
- Object-oriented design (polymorphism, SOLID)
- Performance (N+1 queries, async patterns)
- Testing mindset
- API design best practices

---

## 12-Month Growth Plan

### Month 1-3: OOP Foundations (Junior → Junior+)

**Goal:** Master polymorphism and basic design principles

**Week 1-2: Polymorphism**
- [ ] Study `samples/01-Beginner/PolymorphismBasics/`
- [ ] Read `WHY_THIS_PATTERN.md`
- [ ] Complete exercise: Refactor notification system from if/else to polymorphism
- [ ] Pair programming: Refactor payment processing together
- [ ] Apply: Identify 1 polymorphism opportunity in codebase and refactor

**Success Criteria:**
- Can explain polymorphism without jargon
- Can identify type-checking anti-patterns in code reviews
- Successfully refactored 1 production code example

**Week 3-4: SOLID Principles (Focus on SRP & OCP)**
- [ ] Study `samples/03-Advanced/SOLIDPrinciples/`
- [ ] Read `WHY_THIS_PATTERN.md` 
- [ ] Exercise: Identify SRP violations in codebase
- [ ] Refactor: Break apart a God Class into smaller services
- [ ] Code review: Start reviewing simple PRs with me

**Success Criteria:**
- Can identify Single Responsibility violations
- Understands Open/Closed Principle (polymorphism connection)
- Participated in 3 code reviews

**Week 5-8: Design Patterns Intro**
- [ ] Study Factory pattern (`samples/03-Advanced/DesignPatterns/`)
- [ ] Study Strategy pattern (recognize as polymorphism application)
- [ ] Exercise: Implement payment gateway factory
- [ ] Apply: Use design pattern in current sprint work
- [ ] Read: Start reading "Head First Design Patterns" (optional)

**Success Criteria:**
- Can implement Factory pattern
- Recognizes Strategy pattern usage
- Applied 1 pattern in production code

**Week 9-12: Async/Await Mastery**
- [ ] Study async/await fundamentals
- [ ] Learn Task vs void, ConfigureAwait
- [ ] Exercise: Refactor synchronous code to async
- [ ] Fix: Find and fix async anti-patterns in codebase
- [ ] Deep dive: Understand deadlocks and how to avoid them

**Success Criteria:**
- No more `.Result` or `.Wait()` in code
- Can explain Task lifecycle
- Correctly uses async/await in all new code

**Monthly Review 1-3 Outcomes:**
- [ ] Polymorphism: Proficient
- [ ] SOLID: Basic understanding
- [ ] Design Patterns: 2-3 patterns learned
- [ ] Async: Can use correctly

---

### Month 4-6: Database & Performance (Junior+ → Mid-Level Entry)

**Goal:** Write performant, scalable code

**Week 13-16: Database Optimization**
- [ ] Study N+1 query problem (`samples/98-RealWorld-Problems/03-N-Plus-One-Problem/`)
- [ ] Learn Include/ThenInclude eager loading
- [ ] Learn projection (Select to DTO)
- [ ] Exercise: Audit codebase for N+1 queries
- [ ] Fix: Optimize 3 slow API endpoints

**Success Criteria:**
- Can identify N+1 queries by reading code
- All new code uses Include for related entities
- Improved 3 endpoints from >1s to <200ms

**Week 17-20: Caching Strategies**
- [ ] Study `samples/98-RealWorld-Problems/02-Cache-Strategy/`
- [ ] Learn cache-aside pattern
- [ ] Learn when NOT to cache
- [ ] Exercise: Add caching to product catalog API
- [ ] Monitor: Track cache hit ratios

**Success Criteria:**
- Implemented caching in 1 feature
- Can explain cache invalidation strategies
- Understands TTL trade-offs

**Week 21-24: Performance Profiling**
- [ ] Learn BenchmarkDotNet
- [ ] Study `samples/03-Advanced/PerformanceOptimization/`
- [ ] Profile: Identify bottleneck in slow feature
- [ ] Optimize: Apply Span<T> or ArrayPool where appropriate
- [ ] Document: Write performance analysis report

**Success Criteria:**
- Can use profiler to find bottlenecks
- Has optimized 1 feature with measurable impact (10x+ faster)
- Understands when optimization is premature

**Monthly Review 4-6 Outcomes:**
- [ ] Database: No more N+1 queries
- [ ] Caching: Can design caching strategy
- [ ] Performance: Can profile and optimize

---

### Month 7-9: System Design & Architecture (Mid-Level)

**Goal:** Design scalable systems, not just implement features

**Week 25-28: Microservice Patterns**
- [ ] Study `samples/98-RealWorld-Problems/04-Microservice-Error-Handling/`
- [ ] Learn circuit breaker pattern (Polly)
- [ ] Learn retry with exponential backoff
- [ ] Exercise: Add resilience to external API calls
- [ ] Design: Propose resilience strategy for checkout flow

**Success Criteria:**
- Implemented circuit breaker in 1 service
- Can explain failure modes in distributed systems
- Proposed architectural improvement accepted by team

**Week 29-32: API Design Mastery**
- [ ] Study `docs/code-reviews/02-API-Design-Review/`
- [ ] Learn RESTful conventions deeply
- [ ] Learn API versioning strategies
- [ ] Exercise: Design new API from scratch (with review)
- [ ] Document: Write API design guidelines for team

**Success Criteria:**
- Can design API following REST principles
- Understands API versioning (URL vs header)
- Designed 1 API that passed senior review without major changes

**Week 33-36: Testing Strategy**
- [ ] Learn unit vs integration vs E2E testing
- [ ] Study test pyramid
- [ ] Learn mocking (when to use, when NOT to use)
- [ ] Exercise: Write tests for untested feature
- [ ] Achieve: 80%+ coverage on new code

**Success Criteria:**
- All new code has tests
- Can write tests without over-mocking
- Understands what to test (behavior, not implementation)

**Monthly Review 7-9 Outcomes:**
- [ ] System Design: Can design small-medium features
- [ ] Resilience: Understands failure modes
- [ ] Testing: Tests are habit, not afterthought

---

### Month 10-12: Leadership & Senior Prep

**Goal:** Start mentoring others, lead small projects

**Week 37-40: Code Review Leadership**
- [ ] Review 5+ junior PRs per week
- [ ] Use `docs/mentorship/code-review-checklist.md`
- [ ] Give constructive feedback (teach, don't criticize)
- [ ] Pair program with newer junior on their feature
- [ ] Goal: Help 1 junior complete their first solo feature

**Success Criteria:**
- Regularly reviews others' code
- Feedback is specific and educational
- One junior says "your review helped me learn X"

**Week 41-44: Technical Leadership**
- [ ] Lead small project (2-4 week project)
- [ ] Design architecture, create tasks, review all code
- [ ] Run daily standups for your project
- [ ] Present technical design to team
- [ ] Deliver project on time

**Success Criteria:**
- Led project successfully (on time, high quality)
- Team gave positive feedback on your leadership
- Comfortable running meetings and presenting

**Week 45-48: Production Ownership**
- [ ] Be on-call for 1 week (with senior backup)
- [ ] Debug and fix 1 production incident
- [ ] Write postmortem for incident
- [ ] Propose preventive measures
- [ ] Improve monitoring/alerting

**Success Criteria:**
- Handled production incident calmly
- Root cause analysis was thorough
- Implemented preventive fix

**Week 49-52: Mid-Level Readiness**
- [ ] Portfolio: Create GitHub repos showcasing skills
- [ ] Resume: Update with achievements from this year
- [ ] Mock interview: Practice system design interview
- [ ] 360 review: Get feedback from team
- [ ] Promotion packet: Prepare case for mid-level

**Success Criteria:**
- Ready for mid-level interview (internal or external)
- Portfolio shows growth
- Team supports promotion

**Year-End Review:**
- [ ] Technical: Solid mid-level engineer
- [ ] Leadership: Starting to mentor others
- [ ] Production: Can handle incidents
- [ ] **Result:** Promotion to Mid-Level Engineer 🎉

---

## Weekly 1-on-1 Template

**Date:** [Date]  
**Duration:** 30 minutes

### Check-in (5 min)
- How are you feeling?
- Any blockers this week?
- Workload manageable?

### Progress Review (10 min)
- What did you learn this week?
- Show me what you built
- Any challenges?

### Current Week Tasks (5 min)
- [ ] Task 1: [Specific, measurable]
- [ ] Task 2: [Specific, measurable]
- [ ] Task 3: [Specific, measurable]

### Teaching Moment (5 min)
- Quick lesson on: [Topic based on what they struggled with]
- Resource to study: [Link]
- Exercise to try: [Hands-on practice]

### Career Discussion (5 min)
- How are you feeling about growth plan?
- Any adjustments needed?
- Questions about career path?

**Action Items:**
- [ ] [Mentor] Review their PR by EOD
- [ ] [Mentee] Complete exercise by next 1-on-1
- [ ] [Both] Schedule pair programming session

---

## Monthly Review Template

**Month:** [Number]  
**Date:** [Date]

### Technical Skills Progress

| Skill | Started | Current | Target | On Track? |
|-------|---------|---------|--------|-----------|
| Polymorphism | 2/5 | 4/5 | 4/5 | ✅ Yes |
| Design Patterns | 1/5 | 3/5 | 3/5 | ✅ Yes |
| Database Optimization | 2/5 | 3/5 | 4/5 | ⚠️ Behind |

### Achievements This Month ✨
1. Refactored payment system using Factory pattern
2. Fixed N+1 query reducing API latency by 90%
3. Wrote 15 unit tests for order service

### Challenges 🚧
1. Struggled with async deadlock debugging
2. Underestimated task complexity (3 days became 7)

### Next Month Focus
1. Deep dive on async/await fundamentals
2. Improve estimation accuracy
3. Start code reviewing junior PRs

### Mentee Self-Assessment
**What went well:**
- [Mentee fills this out before meeting]

**What could be better:**
- [Mentee fills this out before meeting]

**Questions/Concerns:**
- [Mentee fills this out before meeting]

### Mentor Feedback
**Strengths you're demonstrating:**
- Great initiative identifying the N+1 query
- Excellent questions during code reviews
- Consistently delivering on time

**Areas to focus on:**
- Practice estimating before starting tasks
- Remember to write tests BEFORE shipping feature
- Don't be afraid to ask for help earlier

**Overall:** On track for mid-level promotion!

---

## Adjusting the Plan

### If They're Ahead of Schedule
- Add stretch goals (learn Blazor, GraphQL, etc.)
- Have them mentor newer junior
- Give them more challenging projects
- Accelerate promotion timeline

### If They're Behind Schedule
- **Don't panic.** Learning isn't linear.
- Identify bottleneck: Is it technical or soft skill?
- Adjust: Maybe they need more pair programming
- Extend timeline: 18 months is fine if needed
- Focus: Quality over speed

### If They're Struggling
- **1-on-1 check-in:** "How can I help?"
- Reduce scope: Maybe they're overwhelmed
- More pair programming: Show, don't just tell
- Different learning style: Try videos instead of reading
- **Remember:** Everyone learns at their own pace

### If They Want to Change Direction
- **Listen:** "What interests you about X?"
- Adjust: Maybe they want backend → frontend
- Incorporate: Add frontend learning to plan
- **Support:** Your job is to help them grow in THEIR direction

---

## Success Metrics

### Technical Milestones
- ✅ Can refactor type checking to polymorphism
- ✅ Can identify and fix N+1 queries
- ✅ Can design RESTful API
- ✅ Can implement design patterns appropriately
- ✅ Can write comprehensive tests
- ✅ Can profile and optimize performance

### Leadership Milestones
- ✅ Reviews code constructively
- ✅ Pairs with newer juniors
- ✅ Leads small projects
- ✅ Handles production incidents
- ✅ Proposes technical improvements

### Soft Skills Milestones
- ✅ Asks for help when stuck (not after 2 days)
- ✅ Gives clear status updates
- ✅ Estimates tasks reasonably
- ✅ Accepts feedback gracefully
- ✅ Communicates clearly in writing

**When all milestones are hit:** Ready for promotion!

---

## Promotion Criteria: Junior → Mid-Level

### Technical Skills (Must Have)
- [ ] Can implement features independently (with design review)
- [ ] Writes performant code (no N+1, proper caching)
- [ ] Writes testable, maintainable code
- [ ] Understands SOLID principles
- [ ] Can apply 3-5 design patterns
- [ ] Can debug production issues with guidance

### Leadership (Nice to Have)
- [ ] Reviews code for other juniors
- [ ] Mentors 1 newer junior
- [ ] Led 1 small project successfully

### Production Ready (Must Have)
- [ ] Shipped 3+ features with minimal bugs
- [ ] Code reviews have <3 critical issues
- [ ] Has handled 1 production incident

**If 8/10 are ✅, recommend for promotion.**

---

## Resources to Share

### This Repository
- `samples/01-Beginner/` - Fundamentals
- `samples/03-Advanced/` - Design patterns, SOLID, performance
- `samples/98-RealWorld-Problems/` - Production scenarios
- `docs/code-reviews/` - Examples of good reviews
- Each sample has `CAREER_IMPACT.md` - Show them career path

### External Resources
- **Books:** Clean Code (Martin), Design Patterns (Gang of Four)
- **Videos:** Pluralsight C# path, YouTube "Nick Chapsas"
- **Practice:** LeetCode Easy/Medium, Project Euler

### Internal Resources
- Pair them with different seniors each month (varied perspectives)
- Invite them to architecture guild meetings
- Share production postmortems (learning from incidents)

---

## Mentor Self-Reflection

**Am I being a good mentor?**

- [ ] Do I have regular 1-on-1s (bi-weekly minimum)?
- [ ] Am I patient when they make mistakes?
- [ ] Do I teach, not just tell?
- [ ] Am I celebrating their wins?
- [ ] Am I giving constructive (not destructive) feedback?
- [ ] Do they feel comfortable asking me questions?
- [ ] Am I adjusting plan based on their feedback?

**If any are ❌, adjust your approach.**

---

## Final Note

**This is a TEMPLATE, not a rigid plan.**

Every developer is different:
- Some pick up OOP quickly, struggle with async
- Some are natural debuggers, struggle with design
- Some learn fast at first, plateau, then accelerate

**Your job:** Adapt this plan to THEIR strengths and growth areas.

**Remember:**
- You're not building a copy of yourself
- You're helping them become the best version of themselves
- The goal is their growth, not your ego

**Good mentoring changes careers. Be the mentor you wish you had.**

