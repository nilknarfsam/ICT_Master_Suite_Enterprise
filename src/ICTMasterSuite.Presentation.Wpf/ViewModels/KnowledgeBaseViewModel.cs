using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.KnowledgeBase.Dtos;
using System.Collections.ObjectModel;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class KnowledgeBaseViewModel(IKnowledgeBaseService knowledgeBaseService) : ObservableObject
{
    public ObservableCollection<KnowledgeBaseArticleDto> Articles { get; } = [];

    [ObservableProperty] private string modelSearch = string.Empty;
    [ObservableProperty] private string testPhaseSearch = string.Empty;
    [ObservableProperty] private string termSearch = string.Empty;
    [ObservableProperty] private KnowledgeBaseArticleDto? selectedArticle;
    [ObservableProperty] private string modelInput = string.Empty;
    [ObservableProperty] private string testPhaseInput = string.Empty;
    [ObservableProperty] private string symptomInput = string.Empty;
    [ObservableProperty] private string solutionInput = string.Empty;
    [ObservableProperty] private string authorInput = "Analista";
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool hasResults;

    [RelayCommand]
    private async Task SearchAsync()
    {
        var result = await knowledgeBaseService.SearchAsync(new SearchKnowledgeBaseRequest(ModelSearch, TestPhaseSearch, TermSearch));
        Articles.Clear();
        foreach (var article in result.Value ?? [])
        {
            Articles.Add(article);
        }

        HasResults = Articles.Count > 0;
        StatusMessage = result.IsSuccess
            ? (HasResults ? $"{Articles.Count} artigo(s) encontrado(s)." : "Nenhum artigo encontrado para os filtros.")
            : result.Message;
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        var result = await knowledgeBaseService.CreateAsync(new CreateKnowledgeBaseArticleRequest(
            ModelInput, TestPhaseInput, SymptomInput, SolutionInput, AuthorInput));

        StatusMessage = result.IsSuccess ? "Artigo criado com sucesso." : result.Message;
        await SearchAsync();
    }

    [RelayCommand]
    private async Task UpdateSelectedAsync()
    {
        if (SelectedArticle is null)
        {
            return;
        }

        var result = await knowledgeBaseService.UpdateAsync(new UpdateKnowledgeBaseArticleRequest(
            SelectedArticle.Id, ModelInput, TestPhaseInput, SymptomInput, SolutionInput, AuthorInput));

        StatusMessage = result.IsSuccess ? "Artigo atualizado." : result.Message;
        await SearchAsync();
    }

    [RelayCommand]
    private async Task ToggleActiveSelectedAsync()
    {
        if (SelectedArticle is null)
        {
            return;
        }

        var result = await knowledgeBaseService.SetActiveStatusAsync(SelectedArticle.Id, !SelectedArticle.IsActive);
        StatusMessage = result.IsSuccess ? "Status do artigo alterado." : result.Message;
        await SearchAsync();
    }

    partial void OnSelectedArticleChanged(KnowledgeBaseArticleDto? value)
    {
        if (value is null)
        {
            return;
        }

        ModelInput = value.Model;
        TestPhaseInput = value.TestPhase;
        SymptomInput = value.Symptom;
        SolutionInput = value.Solution;
        AuthorInput = value.Author;
    }

    public async Task SearchRelatedAsync(string model, string term)
    {
        ModelSearch = model;
        TermSearch = term;
        await SearchAsync();
    }
}
