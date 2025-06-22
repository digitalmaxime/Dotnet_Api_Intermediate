#!/bin/bash

# The URL for your local Schema Registry
SCHEMA_REGISTRY_URL="http://localhost:8081"

echo "Fetching schemas from $SCHEMA_REGISTRY_URL..."

# Get a list of all subjects and store them in an array
SUBJECTS=$(curl -s -X GET "$SCHEMA_REGISTRY_URL/subjects" | jq -r '.[]')

# Check if there are any subjects to delete
if [ -z "$SUBJECTS" ]; then
  echo "No schemas found to delete."
  exit 0
fi

echo "The following schemas (subjects) will be PERMANENTLY deleted:"
echo "$SUBJECTS"
echo ""

# Ask for confirmation before proceeding
read -p "Are you sure? (y/n) " -n 1 -r
echo ""
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Aborting."
    exit 1
fi

# Loop through and delete each subject permanently
for subject in $SUBJECTS; do
  echo -n "Deleting schema: $subject ... "
  curl -s -X DELETE "$SCHEMA_REGISTRY_URL/subjects/$subject?permanent=true" > /dev/null
  echo "Done."
done

# Loop through and delete each subject permanently
for subject in $SUBJECTS; do
  echo -n "Deleting schema: $subject ... "
  curl -s -X DELETE "$SCHEMA_REGISTRY_URL/subjects/$subject?permanent" > /dev/null
  curl -s -X DELETE "$SCHEMA_REGISTRY_URL/subjects/$subject?permanent=true" > /dev/null
  echo "$subject Done."
done

echo ""
echo "All schemas have been successfully deleted."