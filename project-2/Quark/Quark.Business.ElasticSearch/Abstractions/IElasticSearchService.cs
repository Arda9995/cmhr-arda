using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace Quark.Business.ElasticSearch.Abstractions;

public interface IElasticSearchService
{
    Task<bool> IndexDocumentAsync<T>(T document, string indexName) where T : class;
    Task<bool> UpdateDocumentAsync<T>(T document, string id, string indexName) where T : class;
    Task<List<T>> GetAllDocumentsAsync<T>(string indexName) where T : class;

    Task<List<T>> SearchDocumentsAsync<T>(string indexName, Action<QueryDescriptor<T>> query,
        Action<SortOptionsDescriptor<T>> sort = null, int size = 10000) where T : class;

    Task<SearchResponse<T>> SearchDocumentsWithSearchAfterAsync<T>(string indexName, Action<QueryDescriptor<T>> query, Action<SortOptionsDescriptor<T>> sort = null, int size = 10000, List<FieldValue> searchAfter = null) where T : class;
    Task<T> GetDocumentAsync<T>(string id, string indexName = null) where T : class;
    Task<List<T>> GetByIdsDocumentAsync<T>(List<string> ids, string indexName = null) where T : class;
    Task<bool> BulkIndexDocumentsAsync<T>(IEnumerable<T> documents, string indexName) where T : class;
    Task<bool> HealtyElastic();
    Task<bool> DeleteIndexAsync(string indexName);
    Task<bool> DeleteIndexDocuments(string indexName,List<string> ids);
    Task<bool> IsExistDocumentAsync(string id, string indexName);
    Task<bool> BulkDeleteDocumentsAsync<T>(string indexName, IEnumerable<string> ids) where T : class;
}