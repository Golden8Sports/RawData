/****** Script for SelectTopNRows command from SSMS  ******/
SELECT COUNT(*)
  FROM [DonBest].[dbo].[line]
  WHERE
  timeReceived>'2017-11-16 12:22'