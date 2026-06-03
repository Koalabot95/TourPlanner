# Tour Manager: Design & Architecture Protocol

## 1. Executive Summary
This document outlines the core architectural and UI/UX design decisions for the Tour Manager Angular frontend. The primary objectives of this architecture are **maintainability**, **scalability**, and **DRY (Don't Repeat Yourself) principles**. The project is structured to support a seamless transition from a local-storage-based prototype to a fully integrated backend REST API.

---

## 2. Architectural Decisions

### Separation of Concerns (Services vs. Components)
To ensure components remain lightweight and strictly focused on view rendering and user interactions, all heavy logic and data manipulation have been abstracted into injectable Angular Services.

* **`ImageService`:** Encapsulates the logic for converting uploaded files to base64 `data:` URLs, managing storage size limitations, and retrieving preview URLs. This prevents redundant file-reader logic across multiple creation forms.
* **`AuthService`:** Centralizes HTTP calls for user registration and login, preparing the app for JWT token-based authentication.
* **Data Models:** Strict TypeScript interfaces (`Tour`, `TourLog`, `User`) and Enums (`TransportType`, `Difficulty`) are utilized to enforce type safety across the application.

### Template-Driven Forms
The application relies on Angular's Template-Driven forms combined with standard HTML5 validation attributes (`required`, `minlength`, `min`). This approach was chosen for its simplicity in template binding and rapid prototyping, keeping the TypeScript controllers minimal.

---

## 3. UI/UX & Styling Strategy

### Global Utility Classes
To drastically reduce CSS bundle size and component bloat, structural and layout CSS was moved from individual component files to the global `app.scss`. 

| Utility Class | Purpose | Design Benefit |
| :--- | :--- | :--- |
| `.page-container` | Standardizes max-width and margins. | Ensures content never stretches too wide on ultrawide monitors. |
| `.split-layout` | Implements CSS Grid for 2-column layouts. | Automatically collapses to 1-column on mobile screens (`< 768px`). |
| `.card-container` | Implements Auto-fill CSS Grid. | Creates responsive masonry-style card layouts without media queries. |
| `.form-control` | Standardizes input fields. | Overrides native OS styling (e.g., iOS grays) for consistent cross-browser UI. |
| `.placeholder-box`| Replaces empty images/maps. | Provides a consistent empty-state UX using dashed borders and soft grays. |

### Responsive Design
The application is mobile-first. CSS Grid is heavily preferred over Flexbox for macro-layouts (like the split screen and card grids) because it requires fewer wrapper `<div>` elements and handles automatic wrapping and gap spacing natively.

---

## 4. Reusable Component Library

Instead of repeating HTML structures, the application relies on a robust library of custom UI components. This ensures a cohesive visual language and allows global UI updates by editing a single file.

* **`<app-button>`:** Standardizes all interactive triggers. Accepts `variant` inputs (`primary`, `secondary`, `danger`) to enforce a strict color palette and ensures disabled states are visually distinct.
* **`<app-tour-card>` / `<app-tour-log-card>`:** Encapsulates the visual presentation of data entities. Uses Angular's content projection (`<ng-content>`) to allow parent components to inject specific action buttons into the header while maintaining the card's structural integrity.
* **`<app-image-upload>`:** Abstracts the `<input type="file">` mechanics, preview rendering, and validation states away from the main forms.

---

## 5. Form Validation UX

A critical design decision was to improve the user experience of form validation while reducing developer boilerplate.

**The `<app-form-field>` Wrapper**
Rather than writing repetitive `invalid && (dirty || touched)` checks on every single input, the `<app-form-field>` component utilizes Angular's `@ContentChild(NgModel)` to automatically inspect the state of the input projected inside it. 

* **UX Benefit:** Error messages only appear after the user has interacted with the field (`touched`/`dirty`), preventing the screen from being flooded with red text the moment the page loads.
* **Developer Benefit:** Reduces HTML template size by roughly 40% and eliminates copy-paste errors in validation logic.

---

## 6. Future-Proofing (Backend Readiness)

While the current iteration utilizes `localStorage` for rapid prototyping and meeting grading requirements, the architecture is specifically designed to swap to `HttpClient` modules with minimal friction. 

1.  **Isolated Storage Calls:** Because data retrieval is localized to `ngOnInit` methods and saving is localized to specific save routines, migrating to an API will only require swapping `localStorage.setItem()` with `this.http.post()`.
2.  **Async-Ready:** The UI components are already built to handle missing data (using `@if` control flow blocks and optional chaining), meaning they will gracefully handle network latency when asynchronous observables are introduced.