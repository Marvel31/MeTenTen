using MeTenTenMaui.Models;
using System.Text;
using System.Globalization;

namespace MeTenTenMaui.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _tenTensFilePath;
        private readonly string _topicsFilePath;
        private readonly string _feelingExamplesFilePath;

        public FileStorageService()
        {
            var appDataPath = FileSystem.AppDataDirectory;
            _tenTensFilePath = Path.Combine(appDataPath, "tenten_data.txt");
            _topicsFilePath = Path.Combine(appDataPath, "topics_data.txt");
            _feelingExamplesFilePath = Path.Combine(appDataPath, "feeling_examples.txt");
        }

        public async Task SaveTenTensAsync(List<TenTen> tenTens)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== TenTen Data ===");
            
            foreach (var item in tenTens)
            {
                sb.AppendLine($"ID: {item.Id}");
                sb.AppendLine($"Content: {item.Content}");
                sb.AppendLine($"CreatedAt: {item.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"UpdatedAt: {item.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}");
                sb.AppendLine($"UserId: {item.UserId}");
                sb.AppendLine($"UserName: {item.UserName}");
                sb.AppendLine($"TopicId: {item.TopicId}");
                sb.AppendLine($"TopicSubject: {item.TopicSubject}");
                sb.AppendLine($"IsReadByPartner: {item.IsReadByPartner}");
                sb.AppendLine($"ReadByPartnerAt: {item.ReadByPartnerAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}");
                sb.AppendLine($"EmotionTag: {item.EmotionTag ?? ""}");
                sb.AppendLine($"ImportanceLevel: {item.ImportanceLevel?.ToString() ?? ""}");
                sb.AppendLine("---");
            }

            await File.WriteAllTextAsync(_tenTensFilePath, sb.ToString(), Encoding.UTF8);
        }

        public async Task<List<TenTen>> LoadTenTensAsync()
        {
            var tenTens = new List<TenTen>();
            
            if (!File.Exists(_tenTensFilePath))
                return tenTens;

            var content = await File.ReadAllTextAsync(_tenTensFilePath, Encoding.UTF8);
            var lines = content.Split('\n');
            
            TenTen? currentTenTen = null;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine == "=== TenTen Data ===")
                    continue;
                
                if (trimmedLine == "---")
                {
                    if (currentTenTen != null)
                    {
                        tenTens.Add(currentTenTen);
                        currentTenTen = null;
                    }
                    continue;
                }
                
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;
                
                var parts = trimmedLine.Split(new[] { ": " }, 2, StringSplitOptions.None);
                if (parts.Length != 2)
                    continue;
                
                var key = parts[0];
                var value = parts[1];
                
                if (key == "ID")
                {
                    currentTenTen = new TenTen { Id = int.Parse(value) };
                }
                else if (currentTenTen != null)
                {
                    switch (key)
                    {
                        case "Content":
                            currentTenTen.Content = value;
                            break;
                        case "CreatedAt":
                            currentTenTen.CreatedAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                        case "UpdatedAt":
                            if (!string.IsNullOrEmpty(value))
                                currentTenTen.UpdatedAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                        case "UserId":
                            currentTenTen.UserId = int.Parse(value);
                            break;
                        case "UserName":
                            currentTenTen.UserName = value;
                            break;
                        case "TopicId":
                            currentTenTen.TopicId = int.Parse(value);
                            break;
                        case "TopicSubject":
                            currentTenTen.TopicSubject = value;
                            break;
                        case "IsReadByPartner":
                            currentTenTen.IsReadByPartner = bool.Parse(value);
                            break;
                        case "ReadByPartnerAt":
                            if (!string.IsNullOrEmpty(value))
                                currentTenTen.ReadByPartnerAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                        case "EmotionTag":
                            currentTenTen.EmotionTag = string.IsNullOrEmpty(value) ? null : value;
                            break;
                        case "ImportanceLevel":
                            currentTenTen.ImportanceLevel = string.IsNullOrEmpty(value) ? null : int.Parse(value);
                            break;
                    }
                }
            }
            
            return tenTens;
        }

        public async Task SaveTopicsAsync(List<Topic> topics)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Topics Data ===");
            
            foreach (var topic in topics)
            {
                sb.AppendLine($"ID: {topic.Id}");
                sb.AppendLine($"Subject: {topic.Subject}");
                sb.AppendLine($"Description: {topic.Description}");
                sb.AppendLine($"TopicDate: {topic.TopicDate:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"CreatedAt: {topic.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"UpdatedAt: {topic.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}");
                sb.AppendLine($"IsActive: {topic.IsActive}");
                sb.AppendLine("---");
            }

            await File.WriteAllTextAsync(_topicsFilePath, sb.ToString(), Encoding.UTF8);
        }

        public async Task<List<Topic>> LoadTopicsAsync()
        {
            var topics = new List<Topic>();
            
            if (!File.Exists(_topicsFilePath))
                return topics;

            var content = await File.ReadAllTextAsync(_topicsFilePath, Encoding.UTF8);
            var lines = content.Split('\n');
            
            Topic? currentTopic = null;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine == "=== Topics Data ===")
                    continue;
                
                if (trimmedLine == "---")
                {
                    if (currentTopic != null)
                    {
                        topics.Add(currentTopic);
                        currentTopic = null;
                    }
                    continue;
                }
                
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;
                
                var parts = trimmedLine.Split(new[] { ": " }, 2, StringSplitOptions.None);
                if (parts.Length != 2)
                    continue;
                
                var key = parts[0];
                var value = parts[1];
                
                if (key == "ID")
                {
                    currentTopic = new Topic { Id = int.Parse(value) };
                }
                else if (currentTopic != null)
                {
                    switch (key)
                    {
                        case "Subject":
                            currentTopic.Subject = value;
                            break;
                        case "Description":
                            currentTopic.Description = value;
                            break;
                        case "TopicDate":
                            currentTopic.TopicDate = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                        case "CreatedAt":
                            currentTopic.CreatedAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                        case "UpdatedAt":
                            if (!string.IsNullOrEmpty(value))
                                currentTopic.UpdatedAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                        case "IsActive":
                            currentTopic.IsActive = bool.Parse(value);
                            break;
                    }
                }
            }
            
            return topics;
        }

        public async Task<string> ExportMonthDataAsync(int year, int month, List<TenTen> tenTens, List<Topic> topics)
        {
            var fileName = $"tenten_{year:0000}_{month:00}.txt";
            var exportPath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
            var sb = new StringBuilder();
            sb.AppendLine($"=== 10&10 월별 데이터 ({year}년 {month}월) ===");
            sb.AppendLine();
            
            // Topics 추가
            sb.AppendLine("=== Topics ===");
            foreach (var topic in topics)
            {
                sb.AppendLine($"ID: {topic.Id}");
                sb.AppendLine($"Subject: {topic.Subject}");
                sb.AppendLine($"Description: {topic.Description}");
                sb.AppendLine($"TopicDate: {topic.TopicDate:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"CreatedAt: {topic.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"UpdatedAt: {topic.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}");
                sb.AppendLine($"IsActive: {topic.IsActive}");
                sb.AppendLine("---");
            }
            
            sb.AppendLine();
            sb.AppendLine("=== TenTens ===");
            foreach (var item in tenTens)
            {
                sb.AppendLine($"ID: {item.Id}");
                sb.AppendLine($"Content: {item.Content}");
                sb.AppendLine($"CreatedAt: {item.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"UpdatedAt: {item.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}");
                sb.AppendLine($"UserId: {item.UserId}");
                sb.AppendLine($"UserName: {item.UserName}");
                sb.AppendLine($"TopicId: {item.TopicId}");
                sb.AppendLine($"TopicSubject: {item.TopicSubject}");
                sb.AppendLine($"IsReadByPartner: {item.IsReadByPartner}");
                sb.AppendLine($"ReadByPartnerAt: {item.ReadByPartnerAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}");
                sb.AppendLine($"EmotionTag: {item.EmotionTag ?? ""}");
                sb.AppendLine($"ImportanceLevel: {item.ImportanceLevel?.ToString() ?? ""}");
                sb.AppendLine("---");
            }

            await File.WriteAllTextAsync(exportPath, sb.ToString(), Encoding.UTF8);
            return exportPath;
        }

        public async Task<(List<TenTen> tenTens, List<Topic> topics)> ImportDataAsync(string filePath)
        {
            var tenTens = new List<TenTen>();
            var topics = new List<Topic>();
            
            if (!File.Exists(filePath))
                return (tenTens, topics);

            var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            var lines = content.Split('\n');
            
            string currentSection = "";
            TenTen? currentTenTen = null;
            Topic? currentTopic = null;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith("=== Topics ==="))
                {
                    currentSection = "topics";
                    continue;
                }
                
                if (trimmedLine.StartsWith("=== TenTens ==="))
                {
                    currentSection = "tentens";
                    continue;
                }
                
                if (trimmedLine == "---")
                {
                    if (currentSection == "topics" && currentTopic != null)
                    {
                        topics.Add(currentTopic);
                        currentTopic = null;
                    }
                    else if (currentSection == "tentens" && currentTenTen != null)
                    {
                        tenTens.Add(currentTenTen);
                        currentTenTen = null;
                    }
                    continue;
                }
                
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;
                
                var parts = trimmedLine.Split(new[] { ": " }, 2, StringSplitOptions.None);
                if (parts.Length != 2)
                    continue;
                
                var key = parts[0];
                var value = parts[1];
                
                if (currentSection == "topics")
                {
                    if (key == "ID")
                    {
                        currentTopic = new Topic { Id = int.Parse(value) };
                    }
                    else if (currentTopic != null)
                    {
                        switch (key)
                        {
                            case "Subject":
                                currentTopic.Subject = value;
                                break;
                            case "Description":
                                currentTopic.Description = value;
                                break;
                            case "TopicDate":
                                currentTopic.TopicDate = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                                break;
                            case "CreatedAt":
                                currentTopic.CreatedAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                                break;
                            case "UpdatedAt":
                                if (!string.IsNullOrEmpty(value))
                                    currentTopic.UpdatedAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                                break;
                            case "IsActive":
                                currentTopic.IsActive = bool.Parse(value);
                                break;
                        }
                    }
                }
                else if (currentSection == "tentens")
                {
                    if (key == "ID")
                    {
                        currentTenTen = new TenTen { Id = int.Parse(value) };
                    }
                    else if (currentTenTen != null)
                    {
                        switch (key)
                        {
                            case "Content":
                                currentTenTen.Content = value;
                                break;
                            case "CreatedAt":
                                currentTenTen.CreatedAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                                break;
                            case "UpdatedAt":
                                if (!string.IsNullOrEmpty(value))
                                    currentTenTen.UpdatedAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                                break;
                            case "UserId":
                                currentTenTen.UserId = int.Parse(value);
                                break;
                            case "UserName":
                                currentTenTen.UserName = value;
                                break;
                            case "TopicId":
                                currentTenTen.TopicId = int.Parse(value);
                                break;
                            case "TopicSubject":
                                currentTenTen.TopicSubject = value;
                                break;
                            case "IsReadByPartner":
                                currentTenTen.IsReadByPartner = bool.Parse(value);
                                break;
                            case "ReadByPartnerAt":
                                if (!string.IsNullOrEmpty(value))
                                    currentTenTen.ReadByPartnerAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                                break;
                            case "EmotionTag":
                                currentTenTen.EmotionTag = string.IsNullOrEmpty(value) ? null : value;
                                break;
                            case "ImportanceLevel":
                                currentTenTen.ImportanceLevel = string.IsNullOrEmpty(value) ? null : int.Parse(value);
                                break;
                        }
                    }
                }
            }
            
            return (tenTens, topics);
        }

        public async Task SaveFeelingExamplesAsync(List<FeelingExample> examples)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Feeling Examples Data ===");
            
            foreach (var example in examples)
            {
                sb.AppendLine($"ID: {example.Id}");
                sb.AppendLine($"Category: {example.Category}");
                sb.AppendLine($"SubCategory: {example.SubCategory}");
                sb.AppendLine($"Description: {example.Description}");
                sb.AppendLine($"IsDefault: {example.IsDefault}");
                sb.AppendLine($"CreatedAt: {example.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("---");
            }

            await File.WriteAllTextAsync(_feelingExamplesFilePath, sb.ToString(), Encoding.UTF8);
        }

        public async Task<List<FeelingExample>> LoadFeelingExamplesAsync()
        {
            var examples = new List<FeelingExample>();
            
            if (!File.Exists(_feelingExamplesFilePath))
                return examples;

            var content = await File.ReadAllTextAsync(_feelingExamplesFilePath, Encoding.UTF8);
            var lines = content.Split('\n');
            
            FeelingExample? currentExample = null;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine == "=== Feeling Examples Data ===")
                    continue;
                
                if (trimmedLine == "---")
                {
                    if (currentExample != null)
                    {
                        examples.Add(currentExample);
                        currentExample = null;
                    }
                    continue;
                }
                
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;
                
                var parts = trimmedLine.Split(new[] { ": " }, 2, StringSplitOptions.None);
                if (parts.Length != 2)
                    continue;
                
                var key = parts[0];
                var value = parts[1];
                
                if (key == "ID")
                {
                    currentExample = new FeelingExample { Id = int.Parse(value) };
                }
                else if (currentExample != null)
                {
                    switch (key)
                    {
                        case "Category":
                            currentExample.Category = value;
                            break;
                        case "SubCategory":
                            currentExample.SubCategory = value;
                            break;
                        case "Description":
                            currentExample.Description = value;
                            break;
                        case "IsDefault":
                            currentExample.IsDefault = bool.Parse(value);
                            break;
                        case "CreatedAt":
                            currentExample.CreatedAt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                    }
                }
            }
            
            return examples;
        }
    }
}

