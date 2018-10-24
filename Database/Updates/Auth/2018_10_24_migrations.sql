
CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
  `MigrationId` varchar(95) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table nexus_forever_auth.__efmigrationshistory: ~1 rows (approximately)
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT IGNORE INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
	('20181024212728_InitialCreate', '2.1.4-rtm-31024');