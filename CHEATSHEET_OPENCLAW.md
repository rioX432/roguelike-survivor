# OpenClaw Cheat Sheet

## Gateway

```bash
# Status check (most useful command)
openclaw status

# Gateway restart
openclaw gateway restart

# Gateway logs (live tail)
openclaw logs --follow

# Dashboard (browser)
openclaw dashboard

# Health check
openclaw health

# Deep diagnostics
openclaw doctor
```

## Agents

```bash
# List all agents
openclaw agents list

# Add agent
openclaw agents add <id> --model anthropic/claude-sonnet-4-6

# Delete agent
openclaw agents delete <id>

# Set agent identity
openclaw agents set-identity <id> --name "builder-alpha" --emoji "🔨"
```

### Current Agents

| ID | Model | Role |
|---|---|---|
| main | claude-sonnet-4-6 | Default |
| architect | claude-sonnet-4-6 | PM, task decomposition, asset gen |
| builder-alpha | claude-sonnet-4-6 | Player, UI, attacks, scene files |
| builder-beta | claude-sonnet-4-6 | Enemy, data, spawns, prefabs |
| reviewer | claude-haiku-3-5 | Code review, perf check |

## TUI (Terminal UI)

```bash
# Open TUI (main session)
openclaw tui

# Open TUI for specific agent
openclaw tui --session agent:architect:main

# Send initial message
openclaw tui --message "Create the PlayerController script"
```

## Agent (Single Turn)

```bash
# Run one agent turn
openclaw agent --agent architect --message "List all pending tasks"

# Run and deliver response to channel
openclaw agent --agent builder-alpha --message "Implement CameraFollow" --deliver
```

## Config

```bash
# Get config value
openclaw config get agents.defaults
openclaw config get gateway

# Set config value
openclaw config set agents.defaults.workspace "/Users/hackathon/workspace/roguelike-survivor"
openclaw config set agents.defaults.model.primary "anthropic/claude-sonnet-4-6"

# Interactive setup wizard
openclaw configure
openclaw configure --section model
openclaw configure --section gateway
```

## Models & Auth

```bash
# Show model status + auth
openclaw models status

# Set default model
openclaw models set anthropic/claude-sonnet-4-6

# Paste API token
openclaw models auth paste-token

# Setup token via provider CLI
openclaw models auth setup-token

# Auth login flow
openclaw models auth login
```

## Security

```bash
# Security audit
openclaw security audit

# Deep security audit
openclaw security audit --deep
```

## Sessions

```bash
# List sessions
openclaw sessions

# Manage approvals
openclaw approvals list
```

## Skills

```bash
# List available skills
openclaw skills list

# Install skills from ClawHub
npx clawhub search <keyword>
npx clawhub install <skill>
```

## Devices

```bash
# List devices
openclaw devices list

# Approve device
openclaw devices approve <id>
```

## Channels (Telegram)

```bash
# List channels
openclaw channels list

# Channel status
openclaw channels status

# Add/update Telegram bot
openclaw channels add --channel telegram --token "<BOT_TOKEN>" --name "GlitchClaw Agent"

# Enable/disable plugin
openclaw plugins enable telegram
openclaw plugins disable telegram

# View channel logs
openclaw channels logs
```

## Troubleshooting

```bash
# Full reset (keeps CLI)
openclaw reset

# Restart gateway (fixes most issues)
openclaw gateway restart

# Check gateway port
lsof -i :18789

# Kill stale processes
openclaw gateway --force
```
