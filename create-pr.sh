#!/bin/bash

# Create Pull Request for Phase 2
# Run this script after authenticating with: gh auth login

cd /workspace/money-flows/backend

echo "Creating Pull Request for Phase 2: Core Domain Implementation..."

gh pr create \
  --title "Phase 2: Core Domain Implementation - Properties, Expenses, and Tenants" \
  --body-file PR_PHASE_2.md \
  --base main \
  --head feature/phase-2-core-domain

if [ $? -eq 0 ]; then
  echo "✅ Pull Request created successfully!"
  gh pr view --web
else
  echo "❌ Failed to create PR. You can create it manually at:"
  echo "https://github.com/RichardWilliams/money-flows-api/compare/main...feature/phase-2-core-domain"
fi
