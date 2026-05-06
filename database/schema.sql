CREATE EXTENSION IF NOT EXISTS "pgcrypto";

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

CREATE TABLE tours (
    tour_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    start_location VARCHAR(255),
    end_location VARCHAR(255),
    start_date TIMESTAMP NOT NULL, 
    end_date TIMESTAMP NOT NULL,   
    transport_type VARCHAR(50),    -- enum
    distance FLOAT,
    estimated_time FLOAT,
    route_information TEXT,
    map_snapshot_path TEXT,        -- Pfad zum generierten Kartenbild
    popularity INTEGER DEFAULT 0,
    child_friendliness FLOAT DEFAULT 0.0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE tour_logs (
    log_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tour_id UUID REFERENCES tours(tour_id) ON DELETE CASCADE,
    name VARCHAR(100),
    date_time TIMESTAMP NOT NULL,
    comment TEXT,
    difficulty VARCHAR(50),       -- enum
    total_distance FLOAT,
    total_time FLOAT,
    rating INTEGER CHECK (rating >= 1 AND rating <= 5),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE images (
    image_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    log_id UUID REFERENCES tour_logs(log_id) ON DELETE CASCADE, -- Verknüpfung zum Log-Eintrag
    file_path TEXT NOT NULL,       -- Speicherort des Bildes
    caption TEXT,                  
    uploaded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);