curl -X POST "https://localhost:7104/api/todos/work" \
  -H "Content-Type: application/json" \
  -d '{
    "Title": "Title Work",
    "Description": "Work your but off mate!",
    "DueDate": "2025-12-02"
  }'

