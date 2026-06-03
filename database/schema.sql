CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- 1. USERS
CREATE TABLE users (
    user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    bio TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. TOURS
CREATE TABLE tours (
    tour_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    start_location VARCHAR(255) NOT NULL,
    end_location VARCHAR(255) NOT NULL,
    start_date TIMESTAMP NOT NULL, 
    end_date TIMESTAMP NOT NULL,   
    transport_type VARCHAR(50),    -- Wird als String-Enum gemappt
    distance DOUBLE PRECISION,     -- passt zu C# double
    estimated_time DOUBLE PRECISION,
    route_information TEXT,
    map_snapshot_path TEXT,        
    popularity INTEGER DEFAULT 0,
    child_friendliness DOUBLE PRECISION DEFAULT 0.0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. TOUR_LOGS
CREATE TABLE tour_logs (
    log_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tour_id UUID REFERENCES tours(tour_id) ON DELETE CASCADE,
    name VARCHAR(100),
    date_time TIMESTAMP NOT NULL,
    comment TEXT,
    difficulty VARCHAR(50),       
    total_distance DOUBLE PRECISION,
    total_time DOUBLE PRECISION,
    rating INTEGER CHECK (rating >= 1 AND rating <= 5),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 4. IMAGES
CREATE TABLE images (
    image_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    file_path TEXT NOT NULL,       
    caption TEXT,                  
    uploaded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Optionale Fremdschlüssel
    log_id UUID REFERENCES tour_logs(log_id) ON DELETE CASCADE, 
    tour_id UUID REFERENCES tours(tour_id) ON DELETE CASCADE     
);