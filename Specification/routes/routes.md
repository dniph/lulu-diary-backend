# 📘 *## 👤 **Profiles Controller**  [🔗 Profile Model]

```http
POST   /profiles
GET    /profiles/{username}
PATCH  /profiles/{username}
```

**Description:**

* `POST`: Create a new profile (one per authenticated user).
* `GET`: Retrieve profile by username.
* `PATCH`: Update the authenticated user's information.

**Access Rules:**

* 🔒 `POST` requires authentication and can only create one profile per user.
* ✅ `GET` is public (can be accessed by anyone).
* 🔒 `PATCH` is restricted to the authenticated user.erview**

Each section corresponds to a functional controller. For detailed information on the schema and logic, refer to the associated model or controller documentation linked next to each heading.

---

## 👤 **Profiles Controller**  [🔗 Profile Model]

```http
GET    /profiles/{username}
PATCH  /profiles/{username}
```

**Description:**

* `GET`: Retrieve profile by username.
* `PATCH`: Update the authenticated user's information.

**Access Rules:**

* ✅ `GET` is public (can be accessed by anyone).
* 🔒 `PATCH` is restricted to the authenticated user.

---

## 🤝 **Followers Controller**  [🔗 Follower Model]

```http
GET    /profiles/{username}/followers
GET    /profiles/{username}/following
POST   /profiles/{username}/follow
POST   /profiles/{username}/unfollow
```

**Description:**

* Follow or unfollow a profile by username.
* View a profile’s followers or the accounts they follow.

**Access Rules:**

* 🔒 `POST` actions (follow/unfollow) require authentication.
* ✅ `GET` actions are publicly accessible.

---

## 👥 **Friends Controller**  [🔗 Friend Model]

```http
POST   /profiles/{username}/friend-requests
GET    /profiles/{username}/friend-requests
POST   /profiles/{username}/friend-requests/{id}/accept
POST   /profiles/{username}/friend-requests/{id}/reject
GET    /profiles/{username}/friends
```

**Description:**

* Send and manage friend requests by username.
* Accept or reject friend requests.
* View the friend list of a profile.

**Access Rules:**

* 🔒 All endpoints require authentication.
* 🚫 Friend requests **cannot be sent to oneself**.
* ✅ Accept/reject operations can only be performed by the profile receiving the request.
* ✅ `GET /friends` is public.

---

## 📔 **Diaries Controller**  [🔗 Diary Model]

```http
POST   /profiles/{username}/diaries
GET    /profiles/{username}/diaries
GET    /profiles/{username}/diaries/{diaryId}
PATCH  /profiles/{username}/diaries/{diaryId}
DELETE /profiles/{username}/diaries/{diaryId}
```

**Description:**

* Create, view, edit, or delete a diary by username.

**Access Rules:**

* 🔒 Authenticated users can create, update, or delete *their own* diaries.
* ✅ `GET` requests return diaries based on visibility rules:

  * Public diaries: always accessible.
  * Friends-only: accessible if relationship permits.
  * Private: accessible by the owner only.

---

## 💬 **Comments Controller**  [🔗 Comment Model]

```http
POST   /profiles/{username}/diaries/{diaryId}/comments
GET    /profiles/{username}/diaries/{diaryId}/comments
DELETE /profiles/{username}/diaries/{diaryId}/comments/{commentId}
```

**Description:**

* Post or view comments on a diary by username.
* Delete comments (typically by the comment author or diary owner).

**Access Rules:**

* 🔒 `POST` and `DELETE` require authentication.
* ✅ `GET` is public but respects the diary's visibility level.
* 🧑‍⚖️ Only comment authors or diary owners can delete comments.

---

## ❤️ **Reactions Controller**  [🔗 Reaction Model]

```http
POST /api/profiles/{username}/diaries/{diaryId}/react
POST /api/profiles/{username}/diaries/{diaryId}/unreact
GET  /api/profiles/{username}/diaries/{diaryId}/reactions

POST /api/profiles/{username}/diaries/{diaryId}/comments/{commentId}/react
POST /api/profiles/{username}/diaries/{diaryId}/comments/{commentId}/unreact
GET  /api/profiles/{username}/diaries/{diaryId}/comments/{commentId}/reactions
```



**Description:**

* React to or remove a reaction from a diary or a comment by username.
* View all reactions on a diary or comment.
* Supported reaction types: "like", "love", "laugh", "sad", "angry", "surprised".

**Access Rules:**

* 🔒 All `POST` endpoints require authentication.
* ✅ `GET` endpoints are public but respect the diary's visibility level.
* 🚫 Profiles can react only once per target (replaces existing reaction).
* 🧑‍⚖️ Reactions respect the visibility and access control of the target (diary or comment).

---

## 📚 **Feed Controller**  [🔗 Feed Model]

```http
GET /feed
```

**Description:**

* Retrieve a public diary feed from all profiles.

**Access Rules:**

* ✅ Supports both authenticated and unauthenticated users.
* 📖 Returns only *public* diary entries or diaries an authenticated user has access to.

---

### 📌 Final Notes

* Every `POST`, `PATCH`, and `DELETE` route requires authentication unless explicitly stated.
* All visibility restrictions (e.g., private, friends-only) should be handled in middleware or controller guards.

---
