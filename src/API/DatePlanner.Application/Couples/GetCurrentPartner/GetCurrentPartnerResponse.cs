namespace DatePlanner.Application.Couples.GetCurrentPartner;

public sealed record GetCurrentPartnerResponse(
    Guid PartnerId,
    string PartnerName,
    PartnerPreferencesResponse PartnerPreferences);