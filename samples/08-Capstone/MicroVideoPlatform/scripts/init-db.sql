-- ============================================================================
-- Micro-Video Platform - Database Initialization Script
-- ============================================================================
-- This script is automatically executed when the PostgreSQL container starts
-- for the first time. It creates the database schema and seeds initial data.

-- ============================================================================
-- CREATE EXTENSIONS
-- ============================================================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";  -- For full-text search

-- ============================================================================
-- CREATE SCHEMA
-- ============================================================================

-- Videos table
CREATE TABLE IF NOT EXISTS videos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    file_name VARCHAR(500) NOT NULL,
    file_size BIGINT NOT NULL,
    duration_seconds INTEGER,
    resolution VARCHAR(50),
    format VARCHAR(50),
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    thumbnail_url VARCHAR(1000),
    category VARCHAR(100),
    tags TEXT[],
    views_count INTEGER NOT NULL DEFAULT 0,
    likes_count INTEGER NOT NULL DEFAULT 0,
    uploaded_by VARCHAR(255),
    uploaded_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    processed_at TIMESTAMP WITH TIME ZONE,
    last_modified_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    metadata JSONB,
    CONSTRAINT chk_status CHECK (status IN ('Pending', 'Processing', 'Completed', 'Failed'))
);

-- Video processing history table
CREATE TABLE IF NOT EXISTS video_processing_history (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    video_id UUID NOT NULL REFERENCES videos(id) ON DELETE CASCADE,
    status VARCHAR(50) NOT NULL,
    started_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMP WITH TIME ZONE,
    error_message TEXT,
    processing_time_ms BIGINT,
    worker_id VARCHAR(255),
    metadata JSONB
);

-- Video analytics table
CREATE TABLE IF NOT EXISTS video_analytics (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    video_id UUID NOT NULL REFERENCES videos(id) ON DELETE CASCADE,
    predicted_category VARCHAR(100) NOT NULL,
    confidence_score DECIMAL(5,4) NOT NULL,
    analyzed_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    model_version VARCHAR(50),
    features JSONB
);

-- ============================================================================
-- CREATE INDEXES
-- ============================================================================

-- Videos indexes
CREATE INDEX IF NOT EXISTS idx_videos_status ON videos(status);
CREATE INDEX IF NOT EXISTS idx_videos_uploaded_at ON videos(uploaded_at DESC);
CREATE INDEX IF NOT EXISTS idx_videos_category ON videos(category);
CREATE INDEX IF NOT EXISTS idx_videos_uploaded_by ON videos(uploaded_by);
CREATE INDEX IF NOT EXISTS idx_videos_tags ON videos USING GIN(tags);
CREATE INDEX IF NOT EXISTS idx_videos_metadata ON videos USING GIN(metadata);

-- Full-text search index
CREATE INDEX IF NOT EXISTS idx_videos_title_search ON videos USING GIN(to_tsvector('english', title));
CREATE INDEX IF NOT EXISTS idx_videos_description_search ON videos USING GIN(to_tsvector('english', description));

-- Processing history indexes
CREATE INDEX IF NOT EXISTS idx_processing_history_video_id ON video_processing_history(video_id);
CREATE INDEX IF NOT EXISTS idx_processing_history_started_at ON video_processing_history(started_at DESC);

-- Analytics indexes
CREATE INDEX IF NOT EXISTS idx_analytics_video_id ON video_analytics(video_id);
CREATE INDEX IF NOT EXISTS idx_analytics_category ON video_analytics(predicted_category);
CREATE INDEX IF NOT EXISTS idx_analytics_analyzed_at ON video_analytics(analyzed_at DESC);

-- ============================================================================
-- CREATE FUNCTIONS AND TRIGGERS
-- ============================================================================

-- Function to update last_modified_at timestamp
CREATE OR REPLACE FUNCTION update_last_modified_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.last_modified_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger for videos table
DROP TRIGGER IF EXISTS trg_videos_last_modified ON videos;
CREATE TRIGGER trg_videos_last_modified
    BEFORE UPDATE ON videos
    FOR EACH ROW
    EXECUTE FUNCTION update_last_modified_at();

-- Function to increment views count safely
CREATE OR REPLACE FUNCTION increment_video_views(video_uuid UUID)
RETURNS VOID AS $$
BEGIN
    UPDATE videos
    SET views_count = views_count + 1
    WHERE id = video_uuid;
END;
$$ LANGUAGE plpgsql;

-- Function to get video statistics
CREATE OR REPLACE FUNCTION get_video_statistics()
RETURNS TABLE (
    total_videos BIGINT,
    pending_videos BIGINT,
    processing_videos BIGINT,
    completed_videos BIGINT,
    failed_videos BIGINT,
    total_views BIGINT,
    total_likes BIGINT,
    avg_processing_time_ms BIGINT
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        COUNT(*) AS total_videos,
        COUNT(*) FILTER (WHERE status = 'Pending') AS pending_videos,
        COUNT(*) FILTER (WHERE status = 'Processing') AS processing_videos,
        COUNT(*) FILTER (WHERE status = 'Completed') AS completed_videos,
        COUNT(*) FILTER (WHERE status = 'Failed') AS failed_videos,
        COALESCE(SUM(views_count), 0) AS total_views,
        COALESCE(SUM(likes_count), 0) AS total_likes,
        COALESCE(AVG(vph.processing_time_ms)::BIGINT, 0) AS avg_processing_time_ms
    FROM videos v
    LEFT JOIN video_processing_history vph ON v.id = vph.video_id AND vph.completed_at IS NOT NULL;
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- SEED SAMPLE DATA (Development/Demo)
-- ============================================================================

-- Insert sample videos
INSERT INTO videos (id, title, description, file_name, file_size, duration_seconds, resolution, format, status, category, tags, uploaded_by)
VALUES
    (
        '11111111-1111-1111-1111-111111111111',
        'Introduction to Microservices',
        'Learn the basics of microservices architecture and how to build scalable distributed systems.',
        'intro-microservices.mp4',
        52428800,
        1200,
        '1920x1080',
        'mp4',
        'Completed',
        'Education',
        ARRAY['microservices', 'architecture', 'tutorial'],
        'admin@microvideo.com'
    ),
    (
        '22222222-2222-2222-2222-222222222222',
        'C# Advanced Concepts',
        'Deep dive into advanced C# concepts including LINQ, async/await, and performance optimization.',
        'csharp-advanced.mp4',
        73400320,
        1800,
        '1920x1080',
        'mp4',
        'Completed',
        'Education',
        ARRAY['csharp', 'programming', 'advanced'],
        'admin@microvideo.com'
    ),
    (
        '33333333-3333-3333-3333-333333333333',
        'Docker and Kubernetes Guide',
        'Complete guide to containerization with Docker and orchestration with Kubernetes.',
        'docker-k8s-guide.mp4',
        94371840,
        2400,
        '1920x1080',
        'mp4',
        'Processing',
        'DevOps',
        ARRAY['docker', 'kubernetes', 'devops'],
        'admin@microvideo.com'
    ),
    (
        '44444444-4444-4444-4444-444444444444',
        'Event-Driven Architecture Patterns',
        'Explore event-driven architecture patterns with RabbitMQ and messaging best practices.',
        'event-driven-patterns.mp4',
        62914560,
        1500,
        '1920x1080',
        'mp4',
        'Pending',
        'Architecture',
        ARRAY['events', 'messaging', 'patterns'],
        'admin@microvideo.com'
    ),
    (
        '55555555-5555-5555-5555-555555555555',
        'ML.NET Machine Learning Basics',
        'Introduction to machine learning with ML.NET framework and practical examples.',
        'mlnet-basics.mp4',
        83886080,
        2100,
        '1920x1080',
        'mp4',
        'Completed',
        'MachineLearning',
        ARRAY['ml', 'mlnet', 'ai'],
        'admin@microvideo.com'
    )
ON CONFLICT (id) DO NOTHING;

-- Insert sample processing history
INSERT INTO video_processing_history (video_id, status, started_at, completed_at, processing_time_ms, worker_id)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'Completed', NOW() - INTERVAL '2 hours', NOW() - INTERVAL '1 hour 55 minutes', 300000, 'worker-1'),
    ('22222222-2222-2222-2222-222222222222', 'Completed', NOW() - INTERVAL '1 hour', NOW() - INTERVAL '55 minutes', 250000, 'worker-2'),
    ('33333333-3333-3333-3333-333333333333', 'Processing', NOW() - INTERVAL '10 minutes', NULL, NULL, 'worker-1')
ON CONFLICT DO NOTHING;

-- Insert sample analytics
INSERT INTO video_analytics (video_id, predicted_category, confidence_score, model_version)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'Education', 0.9543, 'v1.0.0'),
    ('22222222-2222-2222-2222-222222222222', 'Education', 0.9821, 'v1.0.0'),
    ('55555555-5555-5555-5555-555555555555', 'MachineLearning', 0.8765, 'v1.0.0')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- GRANT PERMISSIONS
-- ============================================================================

-- Grant all privileges on tables to the application user
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO postgres;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO postgres;

-- ============================================================================
-- COMPLETION MESSAGE
-- ============================================================================

DO $$
BEGIN
    RAISE NOTICE '✅ Database initialization complete!';
    RAISE NOTICE '📊 Created tables: videos, video_processing_history, video_analytics';
    RAISE NOTICE '🔍 Created indexes for performance optimization';
    RAISE NOTICE '⚡ Created functions: update_last_modified_at, increment_video_views, get_video_statistics';
    RAISE NOTICE '🌱 Seeded 5 sample videos with processing history and analytics';
END $$;
