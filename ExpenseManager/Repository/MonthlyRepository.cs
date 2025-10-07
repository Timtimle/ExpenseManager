using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ExpenseManager.Models;
using ExpenseManager.Repository.Interface;

namespace ExpenseManager.Repository {
    public class MonthlyRepository : IMonthlyRepository {
        private readonly string dataPath = @"..\..\Data\Monthly";

        public MonthlyRepository() {
            // Ensure directory exists
            if (!Directory.Exists(dataPath)) {
                Directory.CreateDirectory(dataPath);
            }
        }

        public MonthlyReport MonthlyReport {
            get => default;
            set {
            }
        }

        public bool SaveMonthlyData(int year, int month, List<Expense> expenses) {
            try {
                var yearPath = Path.Combine(dataPath, year.ToString());
                if (!Directory.Exists(yearPath)) {
                    Directory.CreateDirectory(yearPath);
                }

                var filePath = Path.Combine(yearPath, $"{month:00}-{year}-expenses.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(expenses, options);
                
                File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public List<Expense> LoadMonthlyData(int year, int month) {
            try {
                var filePath = Path.Combine(dataPath, year.ToString(), $"{month:00}-{year}-expenses.json");
                
                if (!File.Exists(filePath)) {
                    return new List<Expense>();
                }

                var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                
                return JsonSerializer.Deserialize<List<Expense>>(json, options) ?? new List<Expense>();
            } catch (Exception) {
                return new List<Expense>();
            }
        }

        public bool ArchiveMonth(int year, int month) {
            try {
                var currentPath = Path.Combine(dataPath, year.ToString(), $"{month:00}-{year}-expenses.json");
                var archivePath = Path.Combine(dataPath, "Archive", year.ToString(), $"{month:00}-{year}-expenses.json");
                
                var archiveDir = Path.GetDirectoryName(archivePath);
                if (!Directory.Exists(archiveDir)) {
                    Directory.CreateDirectory(archiveDir);
                }

                if (File.Exists(currentPath)) {
                    File.Move(currentPath, archivePath);
                    return true;
                }
                
                return false;
            } catch (Exception) {
                return false;
            }
        }

        public List<MonthlyReport> GetAvailableMonths() {
            var reports = new List<MonthlyReport>();
            
            try {
                if (!Directory.Exists(dataPath)) {
                    return reports;
                }

                var yearDirectories = Directory.GetDirectories(dataPath).Where(d => !Path.GetFileName(d).Equals("Archive", StringComparison.OrdinalIgnoreCase));

                foreach (var yearDir in yearDirectories) {
                    if (int.TryParse(Path.GetFileName(yearDir), out int year)) {
                        var monthFiles = Directory.GetFiles(yearDir, "*-expenses.json");
                        
                        foreach (var file in monthFiles) {
                            var fileName = Path.GetFileNameWithoutExtension(file);
                            var parts = fileName.Split('-');
                            
                            if (parts.Length >= 2 && int.TryParse(parts[0], out int month)) {
                                reports.Add(new MonthlyReport(year, month));
                            }
                        }
                    }
                }
            } catch (Exception) {
                // Return empty list on error
            }
            
            return reports.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).ToList();
        }

        public bool DeleteMonthlyData(int year, int month) {
            try {
                var filePath = Path.Combine(dataPath, year.ToString(), $"{month:00}-{year}-expenses.json");
                
                if (File.Exists(filePath)) {
                    File.Delete(filePath);
                    
                    // Remove year directory if empty
                    var yearDir = Path.Combine(dataPath, year.ToString());
                    if (Directory.Exists(yearDir) && !Directory.EnumerateFileSystemEntries(yearDir).Any()) {
                        Directory.Delete(yearDir);
                    }
                    
                    return true;
                }
                
                return false;
            } catch (Exception) {
                return false;
            }
        }
    }
}