if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Promo_ValidatePromotion]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Promo_ValidatePromotion]
Go

CREATE PROCEDURE dbo.[Promo_ValidatePromotion]
	@PromoCode varchar(100),
	@UserId uniqueidentifier = null
AS

Declare @PromoId Int
Declare @ParticipantId Int

-- Make sure PromoCode is valid
Select @PromoId = Id From rt_Promotions Where PromoCode = @PromoCode
If @PromoId Is Null
Begin
	Print 'Invalid Promotional Code'
	Select Null Id
	Return
End

-- Check if user's already claimed this promotion
Select @ParticipantId = Id From rt_PromotionParticipants Where UserId = @UserId And PromotionId = @PromoId
If @ParticipantId Is Not Null
Begin
	Print 'User already partcipated in promotion.'
	Select @ParticipantId Id
	Return
End

-- Check availability of promotion
Declare @ValidationTime DateTime
Declare @PromoParticipantCount Int

Select @ValidationTime = GETUTCDATE()
Select @PromoParticipantCount = Count(*) From rt_PromotionParticipants Where PromotionId = @PromoId

--Select @PromoParticipantCount
--
--Select *
--	From rt_Promotions p
--	Left Outer Join rt_PromotionParticipants pp On pp.PromotionId = p.Id
--	Where @ValidationTime >= p.StartDate
--		And (p.EndDate Is Null Or @ValidationTime < p.EndDate)
--		And (p.Quantity Is Null Or @PromoParticipantCount < p.Quantity)

If Exists (
	Select *
	From rt_Promotions p
	Left Outer Join rt_PromotionParticipants pp On pp.PromotionId = p.Id
	Where @ValidationTime >= p.StartDate
		And (p.EndDate Is Null Or @ValidationTime < p.EndDate)
		And (p.Quantity Is Null Or @PromoParticipantCount < p.Quantity) )
Begin
	Print 'Promotion Valid, Recording Participant'
	Insert Into rt_PromotionParticipants (
		PromotionId, UserId, Timestamp )
	Values (
		@PromoId, @UserId, @ValidationTime )

	Select Cast(SCOPE_IDENTITY() As Int) Id

	Return
End
Else
Begin
	Print 'Promotion Expired or Full'
	Select Null Id
End

Go

GRANT EXECUTE ON dbo.[Promo_ValidatePromotion] TO [Public]
Go

/*

[Url_GetUrl] 1


*/

