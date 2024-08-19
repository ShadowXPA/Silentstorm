/*
 * Access tokens and Refresh tokens
 * 
 * Example:
 * access_token: 1234
 * token_type: Bearer
 * expires_in: 604800
 * refresh_token: 4321
 * scope: identity guilds.members.read
 */
CREATE TABLE IF NOT EXISTS oauth2 (
	access_token VARCHAR(100) PRIMARY KEY,
	token_type VARCHAR(10) NOT NULL,
	expires_in INT UNSIGNED NOT NULL DEFAULT 0,
	refresh_token VARCHAR(100) NOT NULL DEFAULT '',
	scope VARCHAR(255) NOT NULL DEFAULT ''
);

/*
 * Application properties
 * 
 * Example:
 * id: silentstorm.redirect_uri
 * value?: http://localhost/silentstorm/autorize
 */
CREATE TABLE IF NOT EXISTS property (
	prop_id VARCHAR(100) PRIMARY KEY,
	value TEXT
);

CREATE TABLE IF NOT EXISTS silentstorm_user (
	user_id INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	username VARCHAR(32) NOT NULL UNIQUE,
	discord_id VARCHAR(22)
);

CREATE TABLE IF NOT EXISTS channel (
	channel_id VARCHAR(22) PRIMARY KEY,
	guild_id VARCHAR(22) NOT NULL,
	name VARCHAR(200) NOT NULL
);

CREATE TABLE IF NOT EXISTS project_type (
    proj_type VARCHAR(32) PRIMARY KEY,
    description TEXT
);

CREATE TABLE IF NOT EXISTS project_status (
	proj_status VARCHAR(32) PRIMARY KEY,
	description TEXT
);

CREATE TABLE IF NOT EXISTS project (
    proj_id INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
    title VARCHAR(32) NOT NULL,
    description TEXT,
    proj_type VARCHAR(32),
	proj_status VARCHAR(32),
    user_id INT UNSIGNED NOT NULL,
    created_at DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP),
    finished_at DATETIME,

    CONSTRAINT FK_user_id_project
    FOREIGN KEY (user_id)
    REFERENCES silentstorm_user(user_id)
    ON DELETE CASCADE,

    CONSTRAINT FK_proj_type_project
    FOREIGN KEY (proj_type)
    REFERENCES project_type(proj_type)
    ON DELETE SET NULL,

    CONSTRAINT FK_proj_status_project
    FOREIGN KEY (proj_status)
    REFERENCES project_status(proj_status)
    ON DELETE SET NULL
);

/*
 * Project (project_id) is announced when the status (proj_status)
 * is reached at the date of announcement (announcement_date) on a
 * Channel (channel_id) with a text (announcement)
 */
CREATE TABLE IF NOT EXISTS project_announcement (
	proj_id INT UNSIGNED NOT NULL,
	proj_status VARCHAR(32) NOT NULL,
	channel_id VARCHAR(22) NOT NULL,
    announcement TEXT,
    announcement_date DATETIME,
    was_announced TINYINT(1) NOT NULL DEFAULT 0,

    PRIMARY KEY (proj_id, proj_status),

    CONSTRAINT FK_channel_id_announcement
    FOREIGN KEY (channel_id)
    REFERENCES channel(channel_id)
    ON DELETE CASCADE,

    CONSTRAINT FK_project_id_announcement
    FOREIGN KEY (proj_id)
    REFERENCES project(proj_id)
    ON DELETE CASCADE,

    CONSTRAINT FK_proj_status_announcement
    FOREIGN KEY (proj_status)
    REFERENCES project_status(proj_status)
    ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS song_submission (
	subm_id INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	proj_id INT UNSIGNED NOT NULL,
    user_id INT UNSIGNED NOT NULL,
    uri VARCHAR(255) NOT NULL,
    title VARCHAR(255) NOT NULL,
    is_selected_for_voting TINYINT(1) NOT NULL DEFAULT 0,

    CONSTRAINT UK_submission UNIQUE KEY (proj_id, user_id),

	CONSTRAINT FK_project_id_submission
    FOREIGN KEY (proj_id)
    REFERENCES project(proj_id)
    ON DELETE CASCADE,

    CONSTRAINT FK_user_id_submission
    FOREIGN KEY (user_id)
    REFERENCES silentstorm_user(user_id)
    ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS song_vote (
	proj_id INT UNSIGNED NOT NULL,
    user_id INT UNSIGNED NOT NULL,
	subm_id INT UNSIGNED NOT NULL,

	PRIMARY KEY (proj_id, user_id),

    CONSTRAINT FK_proj_id_vote
    FOREIGN KEY (proj_id)
    REFERENCES project(proj_id)
    ON DELETE CASCADE,

    CONSTRAINT FK_user_id_vote
    FOREIGN KEY (user_id)
    REFERENCES silentstorm_user(user_id)
    ON DELETE CASCADE,

    CONSTRAINT FK_subm_id_vote
    FOREIGN KEY (subm_id)
    REFERENCES song_submission(subm_id)
    ON DELETE CASCADE
);
