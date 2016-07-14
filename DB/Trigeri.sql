CREATE TRIGGER updateForBid
ON Bid
FOR INSERT
AS
BEGIN

	DECLARE @bidID INT;
	DECLARE @userID INT;
	DECLARE @auctionID INT;
	DECLARE @newPrice FLOAT;

	SELECT @bidID = BidID, @userID = KorisnikID, @auctionID = AukcijaID, @newPrice = PonCena
	FROM inserted;	

	UPDATE Aukcija
	SET TrenutnaCena = @newPrice, BidID = @bidID
	WHERE AukcijaID = @auctionID;

	UPDATE Korisnik
	SET BrojTokena = BrojTokena - 1
	WHERE KorisnikID = @userID;
END;

------------------------------------------------------------------------------------------------------------
