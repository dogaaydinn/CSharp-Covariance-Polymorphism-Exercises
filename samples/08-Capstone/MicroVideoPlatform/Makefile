# ============================================================================
# Micro-Video Platform - Makefile
# ============================================================================
# Simplifies common docker-compose operations

.PHONY: help up down restart logs build clean ps status test

# Default target
.DEFAULT_GOAL := help

# ============================================================================
# HELP
# ============================================================================

help: ## Show this help message
	@echo "Micro-Video Platform - Docker Management"
	@echo ""
	@echo "Usage: make [target]"
	@echo ""
	@echo "Targets:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  %-20s %s\n", $$1, $$2}'

# ============================================================================
# DOCKER COMPOSE OPERATIONS
# ============================================================================

up: ## Start all services
	docker-compose up -d
	@echo "✅ All services started!"
	@echo "🌐 Services available at:"
	@echo "   - ApiGateway:        http://localhost:5000"
	@echo "   - Content.API:       http://localhost:5001"
	@echo "   - Analytics:         http://localhost:5003"
	@echo "   - Web.UI:            http://localhost:5004"
	@echo "   - RabbitMQ UI:       http://localhost:15672 (guest/guest)"
	@echo "   - Adminer:           http://localhost:8080"
	@echo "   - Redis Commander:   http://localhost:8081"
	@echo "   - MailHog:           http://localhost:8025"
	@echo "   - Seq:               http://localhost:5341"

down: ## Stop all services
	docker-compose down
	@echo "✅ All services stopped!"

restart: ## Restart all services
	docker-compose restart
	@echo "✅ All services restarted!"

build: ## Build all services
	docker-compose build --no-cache
	@echo "✅ All services built!"

rebuild: ## Rebuild and restart all services
	docker-compose down
	docker-compose build --no-cache
	docker-compose up -d
	@echo "✅ All services rebuilt and started!"

clean: ## Stop services and remove volumes
	docker-compose down -v
	@echo "⚠️  All services stopped and volumes removed!"
	@echo "⚠️  All data has been deleted!"

clean-images: ## Remove all images
	docker-compose down --rmi all
	@echo "✅ All images removed!"

# ============================================================================
# LOGS AND MONITORING
# ============================================================================

logs: ## Show logs from all services
	docker-compose logs -f

logs-api: ## Show logs from Content.API
	docker-compose logs -f content-api

logs-worker: ## Show logs from Processing.Worker
	docker-compose logs -f processing-worker

logs-analytics: ## Show logs from Analytics.Function
	docker-compose logs -f analytics-function

logs-ui: ## Show logs from Web.UI
	docker-compose logs -f web-ui

logs-gateway: ## Show logs from ApiGateway
	docker-compose logs -f apigateway

# ============================================================================
# STATUS AND INSPECTION
# ============================================================================

ps: ## List running services
	docker-compose ps

status: ## Show detailed status of services
	@echo "📊 Service Status:"
	@docker-compose ps --format table
	@echo ""
	@echo "💾 Volume Usage:"
	@docker volume ls | grep microvideo
	@echo ""
	@echo "🌐 Network:"
	@docker network ls | grep microvideo

health: ## Check health of services
	@echo "🏥 Health Check:"
	@curl -s http://localhost:5000/health | jq '.' 2>/dev/null || echo "ApiGateway: ❌ Not responding"
	@curl -s http://localhost:5001/health | jq '.' 2>/dev/null || echo "Content.API: ❌ Not responding"
	@curl -s http://localhost:5003/health | jq '.' 2>/dev/null || echo "Analytics: ❌ Not responding"
	@curl -s http://localhost:5004/health | jq '.' 2>/dev/null || echo "Web.UI: ❌ Not responding"

# ============================================================================
# DATABASE OPERATIONS
# ============================================================================

db-connect: ## Connect to PostgreSQL database
	docker-compose exec postgres psql -U postgres -d microvideo

db-backup: ## Backup PostgreSQL database
	@mkdir -p backups
	docker-compose exec -T postgres pg_dump -U postgres microvideo > backups/backup_$$(date +%Y%m%d_%H%M%S).sql
	@echo "✅ Database backed up to backups/ directory"

db-restore: ## Restore PostgreSQL database (Usage: make db-restore FILE=backup.sql)
	@test -n "$(FILE)" || (echo "❌ Usage: make db-restore FILE=backup.sql" && exit 1)
	docker-compose exec -T postgres psql -U postgres microvideo < $(FILE)
	@echo "✅ Database restored from $(FILE)"

db-migrate: ## Run database migrations
	docker-compose exec content-api dotnet ef database update
	@echo "✅ Migrations applied!"

db-seed: ## Seed database with sample data
	docker-compose exec postgres psql -U postgres -d microvideo -f /docker-entrypoint-initdb.d/init-db.sql
	@echo "✅ Database seeded!"

# ============================================================================
# REDIS OPERATIONS
# ============================================================================

redis-cli: ## Connect to Redis CLI
	docker-compose exec redis redis-cli

redis-flush: ## Flush all Redis cache
	docker-compose exec redis redis-cli FLUSHALL
	@echo "⚠️  Redis cache cleared!"

# ============================================================================
# RABBITMQ OPERATIONS
# ============================================================================

rabbitmq-status: ## Show RabbitMQ status
	docker-compose exec rabbitmq rabbitmqctl status

rabbitmq-queues: ## List RabbitMQ queues
	docker-compose exec rabbitmq rabbitmqctl list_queues

rabbitmq-purge: ## Purge all RabbitMQ queues
	docker-compose exec rabbitmq rabbitmqctl purge_queue video.uploaded
	docker-compose exec rabbitmq rabbitmqctl purge_queue video.processing.completed
	@echo "⚠️  All queues purged!"

# ============================================================================
# TESTING
# ============================================================================

test: ## Run all tests
	@echo "🧪 Running unit tests..."
	dotnet test
	@echo "✅ All tests passed!"

test-integration: ## Run integration tests
	@echo "🧪 Running integration tests..."
	dotnet test --filter Category=Integration
	@echo "✅ Integration tests passed!"

test-api: ## Test API endpoints
	@echo "🧪 Testing API endpoints..."
	@curl -s http://localhost:5001/health | jq '.'
	@curl -s http://localhost:5001/api/videos | jq '.'
	@echo "✅ API tests completed!"

# ============================================================================
# DEVELOPMENT
# ============================================================================

dev: ## Start services in development mode with hot reload
	docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
	@echo "✅ Development environment started with hot reload!"

prod: ## Start services in production mode
	ASPNETCORE_ENVIRONMENT=Production docker-compose up -d
	@echo "✅ Production environment started!"

shell-api: ## Open shell in Content.API container
	docker-compose exec content-api /bin/bash

shell-worker: ## Open shell in Processing.Worker container
	docker-compose exec processing-worker /bin/bash

# ============================================================================
# MAINTENANCE
# ============================================================================

prune: ## Clean up Docker system
	docker system prune -f
	@echo "✅ Docker system pruned!"

update: ## Update all images
	docker-compose pull
	@echo "✅ All images updated!"

# ============================================================================
# QUICK SETUP
# ============================================================================

setup: ## Initial setup (first time only)
	@echo "🚀 Setting up Micro-Video Platform..."
	@cp .env .env.local 2>/dev/null || echo ".env.local already exists"
	@mkdir -p uploads ml-models logs backups
	@echo "📦 Building images..."
	@make build
	@echo "🚀 Starting services..."
	@make up
	@echo "⏳ Waiting for services to be healthy..."
	@sleep 30
	@make health
	@echo "✅ Setup complete! Platform is ready to use."

# ============================================================================
# ALIASES
# ============================================================================

start: up ## Alias for 'up'
stop: down ## Alias for 'down'
reset: clean up ## Alias for 'clean' then 'up'
