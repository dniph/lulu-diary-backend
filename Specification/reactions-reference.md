# 🎭 **Reaction Types Reference**

## Supported Reaction Types

The diary reactions system supports the following 6 reaction types:

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

### Add/Update Reaction
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
