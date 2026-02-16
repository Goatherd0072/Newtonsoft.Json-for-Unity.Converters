using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Newtonsoft.Json.UnityConverters.Tests.Helpers
{
    public class SamplesContentReportTests
    {
        private const string ReportFileName = "Samples-Content-Test-Report.md";
        private const string TestsAsmDefFileName = "Newtonsoft.Json.UnityConverters.Tests.asmdef";
        private static readonly Regex TypeDeclarationRegex = new Regex(@"\b(class|struct|interface)\b", RegexOptions.Compiled);
        private static readonly Regex TestAttributeRegex = new Regex(@"\[(Test|TestCase|UnityTest|TestCaseSource)\b", RegexOptions.Compiled);

        [Test]
        public void ValidateSamplesContentAndWriteReport()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string reportPath = Path.Combine(Application.dataPath, ReportFileName);
            var sampleRoots = FindSampleRoots(projectRoot).ToList();

            Assert.IsNotEmpty(sampleRoots, $"Could not find any sample folder containing '{TestsAsmDefFileName}'.");

            int requiredFailures;
            string markdown = BuildReport(projectRoot, sampleRoots, out requiredFailures);
            File.WriteAllText(reportPath, markdown);

            Assert.Zero(
                requiredFailures,
                $"Samples content checks failed. See report: {ToUnixPath(GetRelativePath(projectRoot, reportPath))}");
        }

        private static IEnumerable<string> FindSampleRoots(string projectRoot)
        {
            var roots = new List<string>
            {
                Path.Combine(projectRoot, "Assets"),
                Path.Combine(projectRoot, "Packages"),
            };
            var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string root in roots)
            {
                foreach (string sampleRoot in FindSampleRootsInRoot(root, found))
                {
                    yield return sampleRoot;
                }
            }

            if (found.Count == 0)
            {
                string packageCacheRoot = Path.Combine(projectRoot, "Library", "PackageCache");
                foreach (string sampleRoot in FindSampleRootsInRoot(packageCacheRoot, found))
                {
                    yield return sampleRoot;
                }
            }
        }

        private static IEnumerable<string> FindSampleRootsInRoot(string root, HashSet<string> found)
        {
            if (!Directory.Exists(root))
            {
                yield break;
            }

            foreach (string asmdefPath in Directory.GetFiles(root, TestsAsmDefFileName, SearchOption.AllDirectories))
            {
                string directory = Path.GetDirectoryName(asmdefPath);
                if (string.IsNullOrEmpty(directory))
                {
                    continue;
                }

                string fullDirectory = Path.GetFullPath(directory);
                if (found.Add(fullDirectory))
                {
                    yield return fullDirectory;
                }
            }
        }

        private static string BuildReport(string projectRoot, List<string> sampleRoots, out int requiredFailures)
        {
            requiredFailures = 0;
            int scriptCount = 0;
            int passedCount = 0;
            int failedCount = 0;
            int folderCount = 0;
            var sampleReports = new List<SampleReport>();

            foreach (string sampleRoot in sampleRoots.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                var sampleReport = new SampleReport
                {
                    SamplePath = ToUnixPath(GetRelativePath(projectRoot, sampleRoot)),
                    FolderReports = new List<FolderReport>(),
                    Errors = new List<string>(),
                };

                if (!Directory.Exists(sampleRoot))
                {
                    sampleReport.Errors.Add($"Sample folder not found: {sampleReport.SamplePath}");
                    failedCount++;
                    requiredFailures++;
                    sampleReports.Add(sampleReport);
                    continue;
                }

                string[] scripts = Directory.GetFiles(sampleRoot, "*.cs", SearchOption.AllDirectories);
                Array.Sort(scripts, StringComparer.OrdinalIgnoreCase);

                if (scripts.Length == 0)
                {
                    sampleReport.Errors.Add($"No .cs files found in sample folder: {sampleReport.SamplePath}");
                    failedCount++;
                    requiredFailures++;
                    sampleReports.Add(sampleReport);
                    continue;
                }

                var folderMap = new Dictionary<string, List<ScriptReport>>(StringComparer.OrdinalIgnoreCase);
                foreach (string scriptPath in scripts)
                {
                    ScriptReport scriptReport = EvaluateScript(sampleRoot, scriptPath);

                    if (!folderMap.ContainsKey(scriptReport.Folder))
                    {
                        folderMap[scriptReport.Folder] = new List<ScriptReport>();
                    }

                    folderMap[scriptReport.Folder].Add(scriptReport);
                    scriptCount++;

                    if (scriptReport.Passed)
                    {
                        passedCount++;
                    }
                    else
                    {
                        failedCount++;
                        requiredFailures++;
                    }
                }

                foreach (KeyValuePair<string, List<ScriptReport>> folder in folderMap.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
                {
                    folderCount++;
                    folder.Value.Sort((a, b) => string.Compare(a.FileName, b.FileName, StringComparison.OrdinalIgnoreCase));
                    sampleReport.FolderReports.Add(new FolderReport
                    {
                        Folder = folder.Key,
                        Scripts = folder.Value,
                    });
                }

                sampleReports.Add(sampleReport);
            }

            var sb = new StringBuilder();
            sb.AppendLine("# Samples Content Unit Test Report (Unity)");
            sb.AppendLine();
            sb.AppendLine($"- Generated at (UTC): {DateTime.UtcNow:O}");
            sb.AppendLine($"- Test class: {typeof(SamplesContentReportTests).FullName}");
            sb.AppendLine();
            sb.AppendLine("## Summary");
            sb.AppendLine();
            sb.AppendLine("| Metric | Value |");
            sb.AppendLine("| --- | ---: |");
            sb.AppendLine($"| Sample folders | {sampleReports.Count} |");
            sb.AppendLine($"| Folders | {folderCount} |");
            sb.AppendLine($"| Script files (.cs) | {scriptCount} |");
            sb.AppendLine($"| Passed | {passedCount} |");
            sb.AppendLine($"| Failed | {failedCount} |");
            sb.AppendLine();

            foreach (SampleReport sampleReport in sampleReports)
            {
                sb.AppendLine($"## Sample: `{sampleReport.SamplePath}`");
                sb.AppendLine();

                if (sampleReport.Errors.Count > 0)
                {
                    foreach (string error in sampleReport.Errors)
                    {
                        sb.AppendLine($"- FAIL: {error}");
                    }

                    sb.AppendLine();
                    continue;
                }

                foreach (FolderReport folderReport in sampleReport.FolderReports)
                {
                    sb.AppendLine($"### Folder: `{folderReport.Folder}`");
                    sb.AppendLine();
                    sb.AppendLine("| Script file | Result | Checks |");
                    sb.AppendLine("| --- | --- | --- |");

                    foreach (ScriptReport scriptReport in folderReport.Scripts)
                    {
                        sb.AppendLine($"| `{scriptReport.FileName}` | {(scriptReport.Passed ? "PASS" : "FAIL")} | {RenderChecks(scriptReport.Checks)} |");
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private static ScriptReport EvaluateScript(string sampleRoot, string scriptPath)
        {
            string content = File.ReadAllText(scriptPath);
            string relativePath = ToUnixPath(GetRelativePath(sampleRoot, scriptPath));
            string folder = ToUnixPath(Path.GetDirectoryName(relativePath) ?? string.Empty);
            if (string.IsNullOrEmpty(folder))
            {
                folder = "(root)";
            }

            var checks = new List<CheckResult>();

            bool hasText = !string.IsNullOrWhiteSpace(content);
            checks.Add(new CheckResult
            {
                Name = "file is not empty",
                Required = true,
                Passed = hasText,
                Detail = hasText ? "ok" : "empty file",
            });

            bool hasTypeDeclaration = TypeDeclarationRegex.IsMatch(content);
            checks.Add(new CheckResult
            {
                Name = "contains type declaration",
                Required = true,
                Passed = hasTypeDeclaration,
                Detail = hasTypeDeclaration ? "ok" : "no class/struct/interface found",
            });

            string fileName = Path.GetFileName(scriptPath);
            if (fileName.EndsWith("Test.cs", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith("Tests.cs", StringComparison.OrdinalIgnoreCase))
            {
                bool hasTestAttribute = TestAttributeRegex.IsMatch(content);
                checks.Add(new CheckResult
                {
                    Name = "contains NUnit test attribute (optional)",
                    Required = false,
                    Passed = hasTestAttribute,
                    Detail = hasTestAttribute ? "ok" : "missing [Test]/[TestCase]/[UnityTest]/[TestCaseSource]",
                });
            }

            return new ScriptReport
            {
                FileName = fileName,
                Folder = folder,
                Checks = checks,
                Passed = checks.Where(check => check.Required).All(check => check.Passed),
            };
        }

        private static string RenderChecks(List<CheckResult> checks)
        {
            return string.Join("<br>", checks.Select(check =>
                $"{check.Name}{(check.Required ? string.Empty : " [OPTIONAL]")}: {(check.Passed ? "PASS" : $"FAIL ({check.Detail})")}"));
        }

        private static string GetRelativePath(string basePath, string fullPath)
        {
            string normalizedBasePath = EnsureDirectorySeparator(Path.GetFullPath(basePath));
            string normalizedFullPath = Path.GetFullPath(fullPath);

            var baseUri = new Uri(normalizedBasePath, UriKind.Absolute);
            var fullUri = new Uri(normalizedFullPath, UriKind.Absolute);

            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString())
                .Replace('/', Path.DirectorySeparatorChar);
        }

        private static string EnsureDirectorySeparator(string path)
        {
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) ||
                path.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                return path;
            }

            return path + Path.DirectorySeparatorChar;
        }

        private static string ToUnixPath(string path)
        {
            return path.Replace('\\', '/');
        }

        private class SampleReport
        {
            public string SamplePath;
            public List<FolderReport> FolderReports;
            public List<string> Errors;
        }

        private class FolderReport
        {
            public string Folder;
            public List<ScriptReport> Scripts;
        }

        private class ScriptReport
        {
            public string FileName;
            public string Folder;
            public bool Passed;
            public List<CheckResult> Checks;
        }

        private class CheckResult
        {
            public string Name;
            public bool Required;
            public bool Passed;
            public string Detail;
        }
    }
}
