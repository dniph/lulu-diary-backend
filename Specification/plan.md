# 📋 Implementation Plan

This plan will guide you through implementing the backend logic for the Lulu Diary project, using the provided documentation in the `backend-spec` folder. Focus on the business logic and SQL query examples in each model and action file.

## 1. Understand the Models
- Review each file in `backend-spec/models/`.
- Pay special attention to the **Business Logic & Example SQL** sections.
- Make notes on how each model relates to others (e.g., how `Followers` and `Friends` connect to `Profiles`).

## 2. Implement Core CRUD Endpoints
- For each model (e.g., Profiles, Diaries, Comments), implement basic Create, Read, Update, Delete endpoints.
- Use the example SQL queries as a guide for filtering and updating data.
- Enforce business logic rules (e.g., uniqueness, ownership, visibility) in your C# code.

## 3. Implement Relationship Logic
- For `Followers` and `Friends`, ensure you:
  - Prevent self-relationships (profiles cannot follow or friend themselves).
  - Prevent duplicates (one follow/friendship per pair).
  - Use the example SQL to check for existing relationships before inserting.

## 4. Implement Action Endpoints
- Use the files in `backend-spec/actions/` as your guide.
- For each action (e.g., follow, unfollow, react, friend request):
  - Follow the business logic steps described.
  - Use the example SQL queries to check, insert, update, or delete as needed.
  - Handle authentication and authorization as described (e.g., only the owner can update their profile).

## 5. Enforce Visibility and Access Rules
- For diaries and comments, always check visibility and ownership before returning or modifying data.
- Use the example SQL for filtering based on visibility and relationships.

## 6. Testing
- Write unit and integration tests for each endpoint.
- Test all business logic, especially edge cases (e.g., duplicate requests, invalid actions).

## 7. Documentation
- Keep the documentation up to date if you make changes to business logic.
- Reference the `backend-spec` files for any questions about expected behavior.