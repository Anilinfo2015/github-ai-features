
# Pull Request Template

## 📝 Title
Provide a clear and descriptive title for your PR.

---
 
## 📄 Description
- **Summary of Changes**: Briefly explain what this PR does.
- **Motivation**: Why are these changes needed?
- **Linked Issues**: Use keywords like `Fixes #123` or `Closes #456`.

---
 
## ✅ Checklist

### **Required (Must Merge)**
These items are non-negotiable and must be confirmed before merging:
- [ ] **Context & Traceability**
  - PR description clearly defines purpose (what/why).
  - Associated ADO work-item is attached.
  - Scope is strictly defined (no mixing fixes or unrelated changes).
- [ ] **Code Correctness**
  - Solid error/exception handling (no silent failures).
  - No anti-patterns or duplicate logic introduced.
  - Adherence to security standards (e.g., input sanitisation).
- [ ] **Testing & Validation**
  - New logic fully covered by unit/integration tests.
  - Regression failures reproduced and covered by new test cases.
  - CI/CD pipeline status checks are green.
- [ ] **Functional Integrity**
  - Consistent behaviour across all expected scenarios.

---
 
### **Strongly Recommended (Good to Have)**
Address before merging; exceptions require a technical debt ticket:
- [ ] **Code Quality & Readability**
  - Code is highly readable with clear, meaningful names.
  - Existing utilities are used; logic is not recreated.
  - Unnecessary comments avoided (only for non-obvious logic).
- [ ] **Architecture & Modularity**
  - Modular implementation (avoid huge classes/methods).
  - No new technical debt introduced (or acknowledged/ticketed).
- [ ] **Testing Depth**
  - Negative test paths and edge cases covered.
- [ ] **Performance & Scalability**
  - Performance impact considered and minimised.
- [ ] **UI/UX**
  - Screenshots (before/after) included for UI changes.
  - UI follows design system/Figma specifications.
  - Accessibility considerations checked.
- [ ] **Documentation**
  - Documentation of new feature/public API completed.
  - Configuration file changes documented.

---
 
## 🔍 Testing
- [ ] Manual testing completed.
- [ ] Automated tests executed successfully.

---
 
## 📎 Additional Notes
Add any extra context, screenshots, or deployment instructions.
