# MCP (Model Context Protocol) Cheat Sheet

## mcporter CLI

```bash
# List all registered servers (with health status)
mcporter list

# List with tool schemas
mcporter list --schema

# List specific server tools
mcporter list github --schema

# Call a tool directly
mcporter call github.search_repositories query="unity mcp"
mcporter call unity.get_scene_objects

# Server health check
mcporter config doctor
```

## Server Management

### Add Server

```bash
# HTTP server
mcporter config add <name> https://example.com/mcp

# Stdio server (command + args)
mcporter config add <name> --command node --args "/path/to/server.js"

# Stdio server with env vars
mcporter config add <name> --command python --args "/path/to/server.py" --env KEY=value
```

### Remove Server

```bash
mcporter config remove <name>
```

### Inspect Server

```bash
mcporter config get <name>
mcporter config get <name> --json
```

### Import from Other Tools

```bash
# Import from Cursor, Claude, Codex, etc.
mcporter config import cursor
mcporter config import cursor --copy
```

## Current Servers

### github (Docker)
- Command: `docker run ghcr.io/github/github-mcp-server`
- Tools: 38 (repos, issues, PRs, files, branches, etc.)
- Env: `GITHUB_PERSONAL_ACCESS_TOKEN`

```bash
# Example calls
mcporter call github.search_repositories query="roguelike unity"
mcporter call github.create_issue owner=rioX432 repo=roguelike-survivor title="Bug" body="desc"
mcporter call github.list_issues owner=rioX432 repo=roguelike-survivor
```

### unity (Node.js)
- Command: `node /Users/hackathon/workspace/mcp-unity/Server~/build/index.js`
- Tools: 30 (scene, assets, scripts, build, etc.)
- Requires: Unity editor running with mcp-unity package installed

```bash
# Example calls
mcporter call unity.get_scene_objects
mcporter call unity.create_script className="PlayerController" namespaceName="RoguelikeSurvivor"
mcporter call unity.run_tests
```

### comfyui (Python)
- Command: `python /Users/hackathon/workspace/comfyui-mcp-server/server.py`
- Env: `COMFYUI_URL=http://127.0.0.1:8188`
- Requires: ComfyUI app running (`/Applications/ComfyUI.app`)

```bash
# Start ComfyUI first
open /Applications/ComfyUI.app
# Then verify
mcporter list comfyui --schema
```

### suno (Python)
- Command: `python -m suno_mcp.server`
- Requires: Playwright Chromium + Suno login session

```bash
# First-time login (one-time)
cd /Users/hackathon/workspace/suno-mcp
/Users/hackathon/.pyenv/versions/3.11.14/bin/python -m playwright open https://suno.com
# Login in browser, close when done

# Verify
mcporter list suno --schema
```

## Adding a New MCP Server

### Step 1: Install the server

```bash
# npm package
npm install -g <package>

# git clone
git clone <repo> /Users/hackathon/workspace/<name>
cd /Users/hackathon/workspace/<name>
npm install && npm run build  # or pip install -r requirements.txt
```

### Step 2: Register with mcporter

```bash
# Node.js server
mcporter config add <name> --command node --args "/path/to/index.js"

# Python server
mcporter config add <name> --command /Users/hackathon/.pyenv/versions/3.11.14/bin/python --args "/path/to/server.py"

# With environment variables
mcporter config add <name> --command node --args "/path/to/index.js" --env API_KEY=xxx
```

### Step 3: Verify

```bash
mcporter list <name> --schema
```

## Config File Location

- mcporter: `~/.mcporter/mcporter.json`
- OpenClaw reads MCP servers from mcporter config automatically

## Daemon (Keep-Alive)

```bash
mcporter daemon start    # Start keep-alive daemon
mcporter daemon status   # Check daemon status
mcporter daemon stop     # Stop daemon
mcporter daemon restart  # Restart daemon
```

## Troubleshooting

```bash
# Server offline?
mcporter list              # Check status
mcporter config doctor     # Validate config

# Docker not running?
docker ps                  # Check Docker
docker pull ghcr.io/github/github-mcp-server  # Re-pull image

# Python server issues?
/Users/hackathon/.pyenv/versions/3.11.14/bin/python --version
pip list | grep <package>

# Rebuild Node.js server
cd /Users/hackathon/workspace/mcp-unity/Server~
npm run build
```
