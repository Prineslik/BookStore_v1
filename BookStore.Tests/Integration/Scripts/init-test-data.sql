-- ============================================
-- ОЧИСТКА (в правильном порядке из-за FK)
-- ============================================
TRUNCATE TABLE "RoleModelUserModel" CASCADE;
TRUNCATE TABLE "PermissionModelRoleModel" CASCADE;
TRUNCATE TABLE "Books" CASCADE;
TRUNCATE TABLE "Users" CASCADE;
TRUNCATE TABLE "Roles" CASCADE;
TRUNCATE TABLE "Permissions" CASCADE;

-- ============================================
-- USERS (фиксированные ID)
-- ============================================
-- Хеш для пароля "password123" (BCrypt)
INSERT INTO "Users" (
    "Id", "ProfilePhotoURL", "UserName", "Email", 
    "EmailConfirmed", "PasswordHash", "PhoneNumberConfirmed", 
    "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount"
) VALUES
    ('11111111-1111-1111-1111-111111111111', 
     'https://example.com/avatar1.png', 'admin', 'admin@test.com', 
     true, '$2a$11$r/rl8BO6wbgZ5obVWltIRu.sH9daMFCXyIjQmily58v66wf/QVm2K', 
     false, false, false, 0),
    
    ('22222222-2222-2222-2222-222222222222', 
     'https://example.com/avatar2.png', 'user', 'user@test.com', 
     true, '$2a$11$r/rl8BO6wbgZ5obVWltIRu.sH9daMFCXyIjQmily58v66wf/QVm2K', 
     false, false, false, 0),
    
    ('33333333-3333-3333-3333-333333333333', 
     '', 'inactive', 'inactive@test.com', 
     false, '$2a$11$r/rl8BO6wbgZ5obVWltIRu.sH9daMFCXyIjQmily58v66wf/QVm2K', 
     false, false, false, 0);

-- ============================================
-- ROLES
-- ============================================
INSERT INTO "Roles" ("Id", "Name") VALUES
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Admin'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'User'),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Manager');

-- ============================================
-- PERMISSIONS
-- ============================================
INSERT INTO "Permissions" ("Id", "Code", "Description") VALUES
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'users:read', 'Read users'),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'users:add', 'Create users'),
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'books:create', 'Create books'),
    ('99999999-9999-9999-9999-999999999999', 'books:delete', 'Delete books');

-- ============================================
-- USER-ROLES (M:N)
-- ============================================
INSERT INTO "RoleModelUserModel" ("RolesId", "UsersId") VALUES
    -- Admin → Admin
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '11111111-1111-1111-1111-111111111111'),
    -- User → User
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '22222222-2222-2222-2222-222222222222'),
    -- User → Manager
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', '22222222-2222-2222-2222-222222222222');

-- ============================================
-- ROLE-PERMISSIONS (M:N)
-- ============================================
INSERT INTO "PermissionModelRoleModel" ("PermissionsId", "RolesId") VALUES
    -- Admin: все права
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'),
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'),
    ('99999999-9999-9999-9999-999999999999', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'),
    
    -- User: только чтение
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'),
    
    -- Manager: чтение + создание книг
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'cccccccc-cccc-cccc-cccc-cccccccccccc'),
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'cccccccc-cccc-cccc-cccc-cccccccccccc');

-- ============================================
-- BOOKS
-- ============================================
INSERT INTO "Books" ("Id", "Title", "Description", "Price", "StockQuantity") VALUES
    ('30000000-0000-0000-0000-000000000001', 
     '1984', 'Dystopian novel', 19.99, 50),
    
    ('30000000-0000-0000-0000-000000000002', 
     'Harry Potter', 'Fantasy novel', 29.99, 30),
    
    ('30000000-0000-0000-0000-000000000003', 
     'The Shining', 'Horror novel', 24.99, 0),
    
    ('30000000-0000-0000-0000-000000000004', 
     'A Brief History of Time', 'Science book', 34.99, 15);

-- ============================================
-- ПРОВЕРКА
-- ============================================
SELECT 'Users' AS table_name, COUNT(*) AS count FROM "Users"
UNION ALL SELECT 'Roles', COUNT(*) FROM "Roles"
UNION ALL SELECT 'Permissions', COUNT(*) FROM "Permissions"
UNION ALL SELECT 'Books', COUNT(*) FROM "Books";