-- =========================================
-- CLEAN DATABASE
-- =========================================

TRUNCATE TABLE
    "AuthorBook",
    "UserBook",
    "PermissionModelRoleModel",
    "RoleModelUserModel",
    "Authors",
    "Books",
    "Permissions",
    "Roles",
    "Users"
RESTART IDENTITY CASCADE;
-- =========================================
-- USERS
-- =========================================

INSERT INTO "Users"
(
    "Id",
    "UserName",
    "NormalizedUserName",
    "Email",
    "NormalizedEmail",
    "EmailConfirmed",
    "PasswordHash",
    "SecurityStamp",
    "ConcurrencyStamp",
    "PhoneNumberConfirmed",
    "TwoFactorEnabled",
    "LockoutEnabled",
    "AccessFailedCount",
    "ProfilePhotoURL",
    "CreateDate",
    "UpdateDate",
    "IsAuthor",
    "AuthorId"
)
VALUES
(
    '11111111-1111-1111-1111-111111111111',
    'testuser',
    'TESTUSER',
    'testuser@example.com',
    'TESTUSER@EXAMPLE.COM',
    FALSE,
    'test-password-hash',
    'test-security-stamp-1',
    'test-concurrency-stamp-1',
    FALSE,
    FALSE,
    FALSE,
    0,
    '',
    '2026-01-01T00:00:00Z',
    NULL,
    FALSE,
    NULL
),
(
    '22222222-2222-2222-2222-222222222222',
    'testauthor',
    'TESTAUTHOR',
    'testauthor@example.com',
    'TESTAUTHOR@EXAMPLE.COM',
    FALSE,
    'test-password-hash',
    'test-security-stamp-2',
    'test-concurrency-stamp-2',
    FALSE,
    FALSE,
    FALSE,
    0,
    '',
    '2026-01-01T00:00:00Z',
    NULL,
    TRUE,
    NULL
)
ON CONFLICT ("Id") DO NOTHING;


-- =========================================
-- ROLES
-- =========================================

INSERT INTO "Roles" ("Id", "Name")
VALUES
(
    '33333333-3333-3333-3333-333333333333',
    'User'
),
(
    '33333333-1234-1234-1234-333333333333',
    'Default user'
),
(
    '44444444-4444-4444-4444-444444444444',
    'Admin'
)
ON CONFLICT ("Id") DO NOTHING;


-- =========================================
-- PERMISSIONS
-- =========================================

INSERT INTO "Permissions" ("Id", "Code", "Description")
VALUES
(
    '55555555-5555-5555-5555-555555555555',
    'Books.Read',
    'Read books'
),
(
    '66666666-6666-6666-6666-666666666666',
    'Books.Create',
    'Create books'
),
(
    '77777777-7777-7777-7777-777777777777',
    'Books.Delete',
    'Delete books'
)
ON CONFLICT ("Id") DO NOTHING;


-- =========================================
-- PERMISSION ROLE
-- =========================================

INSERT INTO "PermissionModelRoleModel"
(
    "PermissionId",
    "RoleId"
)
VALUES
-- User
(
    '55555555-5555-5555-5555-555555555555',
    '33333333-3333-3333-3333-333333333333'
),

-- Admin
(
    '55555555-5555-5555-5555-555555555555',
    '44444444-4444-4444-4444-444444444444'
),
(
    '66666666-6666-6666-6666-666666666666',
    '44444444-4444-4444-4444-444444444444'
),
(
    '77777777-7777-7777-7777-777777777777',
    '44444444-4444-4444-4444-444444444444'
)
ON CONFLICT ("PermissionId", "RoleId") DO NOTHING;


-- =========================================
-- USER ROLE
-- =========================================

INSERT INTO "RoleModelUserModel"
(
    "RoleId",
    "UserId"
)
VALUES
(
    '33333333-3333-3333-3333-333333333333',
    '11111111-1111-1111-1111-111111111111'
),
(
    '44444444-4444-4444-4444-444444444444',
    '22222222-2222-2222-2222-222222222222'
)
ON CONFLICT ("RoleId", "UserId") DO NOTHING;


-- =========================================
-- BOOKS
-- =========================================

INSERT INTO "Books"
(
    "Id",
    "Title",
    "Description",
    "Price",
    "StockQuantity",
    "CreateDate",
    "UpdateDate"
)
VALUES
(
    '88888888-8888-8888-8888-888888888888',
    'The Hobbit',
    'A fantasy novel by J.R.R. Tolkien.',
    29.99,
    10,
    '2026-01-01T00:00:00Z',
    NULL
),
(
    '99999999-9999-9999-9999-999999999999',
    'Clean Code',
    'A handbook of agile software craftsmanship.',
    39.99,
    5,
    '2026-01-01T00:00:00Z',
    NULL
)
ON CONFLICT ("Id") DO NOTHING;


-- =========================================
-- AUTHORS
-- =========================================

INSERT INTO "Authors"
(
    "Id",
    "Name",
    "Bio",
    "PhotoUrl",
    "UserId"
)
VALUES
(
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    'Test Author',
    'Test author biography.',
    NULL,
    '22222222-2222-2222-2222-222222222222'
)
ON CONFLICT ("Id") DO NOTHING;


-- =========================================
-- AUTHOR BOOK
-- =========================================

INSERT INTO "AuthorBook"
(
    "AuthorId",
    "BookId"
)
VALUES
(
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    '88888888-8888-8888-8888-888888888888'
),
(
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    '99999999-9999-9999-9999-999999999999'
)
ON CONFLICT ("AuthorId", "BookId") DO NOTHING;


-- =========================================
-- USER BOOK
-- =========================================

INSERT INTO "UserBook"
(
    "UserId",
    "BookId"
)
VALUES
(
    '11111111-1111-1111-1111-111111111111',
    '88888888-8888-8888-8888-888888888888'
)
ON CONFLICT ("UserId", "BookId") DO NOTHING;

-- =========================================
-- VERIFY DATABASE SEED
-- =========================================

SELECT 'Users' AS "TableName", COUNT(*) AS "RecordsCount"
FROM "Users"

UNION ALL

SELECT 'Roles', COUNT(*)
FROM "Roles"

UNION ALL

SELECT 'Permissions', COUNT(*)
FROM "Permissions"

UNION ALL

SELECT 'Books', COUNT(*)
FROM "Books"

UNION ALL

SELECT 'Authors', COUNT(*)
FROM "Authors"

UNION ALL

SELECT 'RoleModelUserModel', COUNT(*)
FROM "RoleModelUserModel"

UNION ALL

SELECT 'PermissionModelRoleModel', COUNT(*)
FROM "PermissionModelRoleModel"

UNION ALL

SELECT 'UserBook', COUNT(*)
FROM "UserBook"

UNION ALL

SELECT 'AuthorBook', COUNT(*)
FROM "AuthorBook"

ORDER BY "TableName";
-- =========================================
-- VERIFY FOREIGN KEYS
-- =========================================

SELECT
    'UserRole' AS "Relation",
    COUNT(*) AS "InvalidRecords"
FROM "RoleModelUserModel" ur
LEFT JOIN "Users" u ON u."Id" = ur."UserId"
LEFT JOIN "Roles" r ON r."Id" = ur."RoleId"
WHERE u."Id" IS NULL OR r."Id" IS NULL

UNION ALL

SELECT
    'PermissionRole',
    COUNT(*)
FROM "PermissionModelRoleModel" pr
LEFT JOIN "Permissions" p ON p."Id" = pr."PermissionId"
LEFT JOIN "Roles" r ON r."Id" = pr."RoleId"
WHERE p."Id" IS NULL OR r."Id" IS NULL

UNION ALL

SELECT
    'UserBook',
    COUNT(*)
FROM "UserBook" ub
LEFT JOIN "Users" u ON u."Id" = ub."UserId"
LEFT JOIN "Books" b ON b."Id" = ub."BookId"
WHERE u."Id" IS NULL OR b."Id" IS NULL

UNION ALL

SELECT
    'AuthorBook',
    COUNT(*)
FROM "AuthorBook" ab
LEFT JOIN "Authors" a ON a."Id" = ab."AuthorId"
LEFT JOIN "Books" b ON b."Id" = ab."BookId"
WHERE a."Id" IS NULL OR b."Id" IS NULL;