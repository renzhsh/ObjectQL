-- Create SYS_USER
create table SYS_USER
(
  id                     VARCHAR2(32) not null,
  user_name              NVARCHAR2(40) not null,
  password               VARCHAR2(64) not null,
  sex                    NUMBER,
  type                   NUMBER default 0 not null,
  access_failed_count    NUMBER default 0 not null,
  email                  VARCHAR2(30),
  email_confirmed        NUMBER default 0 not null,
  lockout_enabled        NUMBER default 1 not null,
  lockout_end_date_utc   DATE,
  phone_number           VARCHAR2(30),
  phone_number_confirmed NUMBER default 0 not null,
  security_stamp         NVARCHAR2(64),
  two_factor_enabled     NUMBER default 0 not null,
  create_date            DATE default SYSDATE not null,
  real_name              NVARCHAR2(20),
  id_card_num            VARCHAR2(18),
  organ                  VARCHAR2(32) not null,
  status                 NUMBER(1),
  job                    NVARCHAR2(20),
  office_phone           VARCHAR2(16)
);

-- Create SYS_ROLE
create table SYS_ROLE
(
  id          VARCHAR2(32) not null,
  name        NVARCHAR2(30) not null,
  create_user VARCHAR2(30),
  create_date DATE default SYSDATE not null,
  type        VARCHAR2(32) default 0 not null,
  state       NUMBER(3) default 0 not null,
  sort        NUMBER(4) default 0 not null,
  descs       NVARCHAR2(200),
  region      VARCHAR2(32)
);

-- Create SYS_USER_ROLE
create table SYS_USER_ROLE
(
  id          VARCHAR2(32) not null,
  user_id     VARCHAR2(32),
  role_id     VARCHAR2(32),
  create_date DATE default SYSDATE not null
);

-- Create SYS_REAL_USER
create table SYS_REAL_USER
(
  user_id		VARCHAR2(32) not null,
  real_user_id	VARCHAR2(32),
  id_card_num	VARCHAR2(32),
  real_name		VARCHAR2(32),
  state			number(2),
  create_date	DATE default SYSDATE not null
);
