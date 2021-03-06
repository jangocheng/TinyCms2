﻿CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [Log] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [Customer] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Username] ON [Customer] ([Username] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_CustomerGuid] ON [Customer] ([CustomerGuid] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_SystemName] ON [Customer] ([SystemName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_GenericAttribute_EntityId_and_KeyGroup] ON [GenericAttribute] ([EntityId] ASC, [KeyGroup] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [QueuedEmail] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [Language] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_NewsletterSubscription_Email] ON [NewsletterSubscription] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PollAnswer_PollId] ON [PollAnswer] ([PollId] ASC)
GO

--CREATE NONCLUSTERED INDEX [IX_PostTag_Name] ON [PostTag] ([Name] ASC)
--GO

CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Slug] ON [UrlRecord] ([Slug] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Custom_1] ON [UrlRecord] ([EntityId] ASC, [EntityName] ASC, [LanguageId] ASC, [IsActive] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_AclRecord_EntityId_EntityName] ON [AclRecord] ([EntityId] ASC, [EntityName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_DisplayOrder] ON [Category] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_ParentCategoryId] ON [Category] ([ParentCategoryId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Post_Published] ON [Post] ([Published] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Post_ShowOnHomepage] ON [Post] ([ShowOnHomepage] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PCM_Post_and_Category] ON [Post_Category_Mapping] ([CategoryId] ASC, [PostId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Post_Deleted_and_Published] ON [Post] ([Published] ASC, [Deleted] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PostTag_Name] ON [PostTag] ([Name] ASC)
GO