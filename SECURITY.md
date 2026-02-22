# SECURITY.md — Hackathon Security Policy

## API Keys & Secrets

- API keys (Anthropic, GitHub PAT, Telegram Bot Token) are configured in mcporter/OpenClaw config ONLY
- NEVER echo, print, log, or expose any API key or token in:
  - Terminal output
  - Git commits
  - Chat messages
  - Log files
  - Code comments
- If a key is accidentally exposed, report to human immediately for rotation

## OpenClaw Gateway

- Gateway MUST bind to loopback (127.0.0.1) only
- Auth token mode is mandatory — never set auth to "none"
- Port 18789 must not be exposed to LAN or internet
- After hackathon: revoke all API keys and tokens immediately

## Docker / OpenClaw Isolation

- Docker containers run as non-root
- Volume mounts limited to Unity project directory only
- No network access beyond localhost unless explicitly needed
- Container cleanup after hackathon

## Git & GitHub

- .gitignore must exclude: .env, *.key, credentials.*, openclaw.json
- GitHub PAT has minimal scopes (repo, workflow only)
- PAT must be revoked within 24h after hackathon ends
- Never push to main without review

## File System

- Agents operate ONLY within:
  - `/Users/hackathon/.openclaw/workspace/`
  - Unity project directory
- No access to other users home directories
- No access to system directories
- `trash` preferred over `rm` for recoverable deletion

## Destructive Operations (REQUIRE HUMAN APPROVAL)

- `rm -rf` / `rm -r` on any directory
- `git reset --hard` / `git push --force`
- Docker container/image removal
- Any operation affecting files outside workspace
- Sending data to external services (except GitHub, ComfyUI local)

## Hackathon Completion Checklist

- [ ] Revoke Anthropic API key
- [ ] Revoke GitHub PAT
- [ ] Revoke Telegram Bot Token (@BotFather → /deletebot or /revoke)
- [ ] Stop and remove Docker containers
- [ ] Stop OpenClaw gateway daemon
- [ ] Clear mcporter credentials
- [ ] Review git history for accidentally committed secrets
