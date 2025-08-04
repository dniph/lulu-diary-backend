# 🎭 **Reaction Types Reference**

## Supported Reaction Types

The diary and comment reactions system supports the following 6 reaction types:

| Type | Emoji | Label | Description |
|------|-------|-------|-------------|
| `like` | 👍 | Me gusta | Positive reaction |
| `love` | ❤️ | Me encanta | Strong positive reaction |
| `laugh` | 😄 | Divertido | Funny/amusing reaction |
| `sad` | 😢 | Triste | Sad/sympathetic reaction |
| `angry` | 😠 | Enojado | Angry/frustrated reaction |
| `surprised` | 😮 | Sorprendido | Surprised/shocked reaction |

## Frontend Integration Example

```javascript
const reactionTypes = [
  { type: 'like', emoji: '👍', label: 'Me gusta' },
  { type: 'love', emoji: '❤️', label: 'Me encanta' },
  { type: 'laugh', emoji: '😄', label: 'Divertido' },
  { type: 'sad', emoji: '😢', label: 'Triste' },
  { type: 'angry', emoji: '😠', label: 'Enojado' },
  { type: 'surprised', emoji: '😮', label: 'Sorprendido' }
];
```

## API Usage

### Diary Reactions
```http
POST /api/profiles/{username}/diaries/{diaryId}/react
POST /api/profiles/{username}/diaries/{diaryId}/unreact
GET  /api/profiles/{username}/diaries/{diaryId}/reactions
```

### Comment Reactions
```http
POST /api/profiles/{username}/diaries/{diaryId}/comments/{commentId}/react
POST /api/profiles/{username}/diaries/{diaryId}/comments/{commentId}/unreact
GET  /api/profiles/{username}/diaries/{diaryId}/comments/{commentId}/reactions
```

### Request Body Example
```http
POST /api/profiles/{username}/diaries/{diaryId}/react
Content-Type: application/json

{
  "reactionType": "love"
}
```

### Validation
- All reaction types are **case-insensitive** (`"LOVE"`, `"Love"`, `"love"` are all valid)
- Only the 6 types listed above are accepted
- Invalid types will return a `400 Bad Request` with validation error message
- Reactions are stored in lowercase in the database for consistency

## Business Rules
- Each profile can have **only one reaction per diary**
- New reactions **replace** existing ones
- Duplicate reactions are **idempotent** (no error, same result)
