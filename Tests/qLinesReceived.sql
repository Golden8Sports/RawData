/****** Script for SelectTopNRows command from SSMS  ******/
SELECT COUNT(*)
  FROM [DonBest].[dbo].[message_received]
  where time_received>'2017-11-16 12:31'
  