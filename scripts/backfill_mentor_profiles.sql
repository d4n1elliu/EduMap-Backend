-- Backfill demo mentors. Safe to rerun.
-- Gender: Female=0, Male=1, NonBinary=2, PreferNotToSay=3
-- Course: InformationTechnology=0, ComputerScience=1, Business=2, Law=3, Science=4, Engineering=5,
--         Communications=6, Architecture=7, Health=8, Mathematics=9, InternationalStudies=10, Education=11

UPDATE "Users" AS u
SET "Gender" = v.gender,
    "Course" = v.course
FROM (VALUES
    ('emma-janice@edumap.com',        0, 1),
    ('alex-chen@edumap.com',          1, 2),
    ('sarah-williams@edumap.com',     0, 3),
    ('michael-rodriguez@edumap.com',  1, 0),
    ('danny-lim@edumap.com',          1, 5),
    ('brenden-yung@edumap.com',       1, 0),
    ('jennie-patel@edumap.com',       0, 5),
    ('james-o''connor@edumap.com',    1, 6),
    ('hannah-kim@edumap.com',         0, 3),
    ('carlos-martinez@edumap.com',    1, 10),
    ('emily-zhang@edumap.com',        0, 8),
    ('sung-jing-woo@edumap.com',      1, 0),
    ('johnny-zhang@edumap.com',       1, 10),
    ('victor-zhong@edumap.com',       1, 7),
    ('hector-lim@edumap.com',         1, 9),
    ('david-lee@edumap.com',          1, 10),
    ('mia-su@edumap.com',             0, 11),
    ('jay-kim@edumap.com',            1, 11)
) AS v(email, gender, course)
WHERE lower(u."Email") = v.email
  AND (u."Gender" IS DISTINCT FROM v.gender OR u."Course" IS DISTINCT FROM v.course);

SELECT "Email", "Gender", "Course"
FROM "Users"
WHERE lower("Email") LIKE '%@edumap.com'
ORDER BY lower("Email");
