/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [id_message_received]
      ,[message_received]
      ,[time_received]
  FROM [DonBest].[dbo].[message_received]
  where time_received>'2017-11-16 12:22' and time_received<'2017-11-16 12:27'
  order by message_received