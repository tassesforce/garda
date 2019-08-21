CREATE TABLE USER_AUTH(
  LOGIN VARCHAR(100) NOT NULL,
  PWD VARCHAR(100) NOT NULL,
  ACCOUNT_TYPE VARCHAR(100) NOT NULL,
  USER_ID VARCHAR(500) NOT NULL,
  DATE_C DATE,
  DATE_L_C DATE,
  REVOKED_USER BOOL,
  PHONE_NUMBER VARCHAR(200),
  MAIL_ADRESS VARCHAR(200),
  CONSTRAINT PK_USER_AUTH PRIMARY KEY(USER_ID)
);
CREATE TABLE ROLE(
  ID_ROLE VARCHAR(50) NOT NULL,
  LABEL VARCHAR(200) NOT NULL,
  DESCRIPTION VARCHAR(500),
  CONSTRAINT PK_ROLE PRIMARY KEY (ID_ROLE)
);
CREATE TABLE USER_ROLES(
  USER_ID VARCHAR(500) NOT NULL,
  ID_ROLE VARCHAR(50) NOT NULL,
  CONSTRAINT PK_USER_ROLES PRIMARY KEY (USER_ID, ID_ROLE),
  CONSTRAINT FK_ROLE FOREIGN KEY (ID_ROLE) REFERENCES ROLE(ID_ROLE)
);
CREATE TABLE CLIENT_APP(
  CLIENT_ID VARCHAR(100) NOT NULL,
  CLIENT_SECRET VARCHAR(100) NOT NULL,
  CONSTRAINT PK_CLIENT_APP PRIMARY KEY (CLIENT_ID, CLIENT_SECRET)
);
CREATE TABLE REVOKED_TOKEN(
  REFRESH_TOKEN VARCHAR(2000) NOT NULL,
  REVOCATION_DATE DATE,
  CONSTRAINT PK_REVOKED_TOKEN PRIMARY KEY (REFRESH_TOKEN)
);

insert into CLIENT_APP(CLIENT_ID, CLIENT_SECRET) values ('56784348-45b4-48f1-a245-12f5d4a04a05', '73c18669-0282-45d8-82c5-d7a29ede9400');

insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('getAgencyAccount', 'Droit a l''authentification sur l''application', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('postAgencyAccount', 'Droit de gerer son compte', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('getCompanyAccount', 'Droit a l''authentification sur l''application', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('postCompanyAccount', 'Droit de gerer son compte', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('getCandidateAccount', 'Droit a l''authentification sur l''application', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('postCandidateAccount', 'Droit de gerer son compte', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('listAccounts', 'Droit a la liste des comptes', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('putAgencyAccount', 'Droit a la maj comptes agence', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('putCompanyAccount', 'Droit a la maj des comptes entreprise', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('putCandidateAccount', 'Droit a la maj des comptes candidats', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('deleteAgencyAccount', 'Droit a la suppression de comptes agence', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('deleteCompanyAccount', 'Droit a la suppression de comptes entreprise', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('deleteCandidateAccount', 'Droit a la suppression de comptes candidats', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('getCollaboratorAccount', 'Droit a la récupération d''un compte collaborateur', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('postCollaboratorAccount', 'Droit a l''ajout d''un compte collaborateur', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('putCollaboratorAccount', 'Droit a la mise a jour d''un compte collaborateur', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('deleteCollaboratorAccount', 'Droit a la suppression de comptes collaborateur', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('listCollaboratorAccounts', 'Droit a la recherche de comptes collaborateurs', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('getMission', 'Droit a la récupération d''une mission', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('postMission', 'Droit a l''ajout d''une mission', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('putMission', 'Droit a la mise a jour d''une mission', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('deleteMission', 'Droit a la suppression de missions', 'Role de test');
insert into ROLE (ID_ROLE, LABEL, DESCRIPTION) VALUES ('listMission', 'Droit a la recherche de missions', 'Role de test');