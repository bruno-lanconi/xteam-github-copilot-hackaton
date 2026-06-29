DOCKER ?= docker
DOCKERFILE ?= Bookstore/Dockerfile
BUILD_CONFIGURATION ?= Release

.PHONY: help test static-analysis lint analyze

help:
	@echo "Available targets:"
	@echo "  make test            - Run unit tests in container"
	@echo "  make static-analysis - Run static analysis in container"
	@echo "  make lint            - Run linting checks in container"
	@echo "  make analyze         - Run full analysis pipeline (format + lint + build)"

test:
	$(DOCKER) build \
		--file $(DOCKERFILE) \
		--target test \
		--build-arg BUILD_CONFIGURATION=$(BUILD_CONFIGURATION) \
		.

static-analysis:
	$(DOCKER) build \
		--file $(DOCKERFILE) \
		--target static-analysis \
		--build-arg BUILD_CONFIGURATION=$(BUILD_CONFIGURATION) \
		.

lint:
	$(DOCKER) build \
		--file $(DOCKERFILE) \
		--target lint \
		--build-arg BUILD_CONFIGURATION=$(BUILD_CONFIGURATION) \
		.

analyze:
	$(DOCKER) build \
		--file $(DOCKERFILE) \
		--target analyze \
		--build-arg BUILD_CONFIGURATION=$(BUILD_CONFIGURATION) \
		.
