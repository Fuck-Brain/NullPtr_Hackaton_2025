using Back.Application.Dtos;

namespace Back.Application;

public class AnalyticsClient(HttpClient http) {
    public async Task<ClassificationResponse> ClassifyAsync(ClassificationRequest req, CancellationToken ct = default) {
        var resp = await http.PostAsJsonAsync("", req, ct);
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync<ClassificationResponse>(cancellationToken: ct);
        return data!;
    }
}
