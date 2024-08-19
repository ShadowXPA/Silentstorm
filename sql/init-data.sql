/*
 * Silentstorm initial users
 *
 * The 2 initial users will not be deleted (unless you delete it from the database...)
 */
-- Your initial user
INSERT INTO silentstorm_user(username, discord_id) VALUES('[DISCORD_USERNAME]', '[DISCORD_ID]');
-- The Silentstorm Discord bot must be inserted too
INSERT INTO silentstorm_user(username, discord_id) VALUES('Silentstorm', '[DISCORD_APPLICATION_ID]');

/*
 * Silentstorm properties
 *
 * Most of these properties can be found in your Discord application's developer page
 */
-- Discord application's OAuth 2.0 authorization URI
INSERT INTO property(prop_id, value) VALUES('silentstorm.oauth2', 'https://discord.com/oauth2/authorize?client_id=[YOUR_CLIENT_ID]&response_type=code&redirect_uri=[YOUR_REDIRECT_URI]&scope=identify');
-- Discord application's OAuth 2.0 authorization URI (Development version - IWebHostEnvironment.IsDevelopment())
INSERT INTO property(prop_id, value) VALUES('dev.silentstorm.oauth2', 'https://discord.com/oauth2/authorize?client_id=[YOUR_CLIENT_ID]&response_type=code&redirect_uri=[YOUR_REDIRECT_URI]&scope=identify');
-- Discord's API version
INSERT INTO property(prop_id, value) VALUES('discord.api-version', 'v10');
-- Discord's client ID
INSERT INTO property(prop_id, value) VALUES('silentstorm.client_id', '[YOUR_CLIENT_ID]');
-- Discord's client secret
INSERT INTO property(prop_id, value) VALUES('silentstorm.client_secret', '[YOUR_CLIENT_SECRET]');
-- Redirect URI for Discord's OAuth 2.0 (Example: http://localhost:3000/authorize)
INSERT INTO property(prop_id, value) VALUES('silentstorm.redirect_uri', '[YOUR_REDIRECT_URI]');
-- Redirect URI for Discord's OAuth 2.0 (Development version - IWebHostEnvironment.IsDevelopment())
INSERT INTO property(prop_id, value) VALUES('dev.silentstorm.redirect_uri', '[YOUR_REDIRECT_URI]');
--
-- Status when a project is created
-- INSERT INTO property(prop_id, value) VALUES('silentstorm.project.create-status', 'CREATED');
-- Status the project needs to be to accept song submissions
-- INSERT INTO property(prop_id, value) VALUES('silentstorm.project.submission-status', 'SUBMISSION');
-- Status the project needs to be to accept voting submissions
-- INSERT INTO property(prop_id, value) VALUES('silentstorm.project.voting-status', 'VOTING');

/*
 * Project types
 */
INSERT INTO project_type(proj_type, description) VALUES('MEP', 'Multi Editor Project');
INSERT INTO project_type(proj_type) VALUES('WORM');

/*
 * Project statuses
 */
INSERT INTO project_status(proj_status, description) VALUES('CREATED', 'Project has been created');
INSERT INTO project_status(proj_status, description) VALUES('SUBMISSION', 'Project is accepting song submissions');
INSERT INTO project_status(proj_status, description) VALUES('VOTING', 'Project is accepting votes');
INSERT INTO project_status(proj_status, description) VALUES('DEVELOPING', 'Project is being developed');
INSERT INTO project_status(proj_status, description) VALUES('FINISHED', 'Project has been finished');
