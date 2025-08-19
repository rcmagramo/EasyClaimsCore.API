using EasyClaimsCore.API.Models.Responses;
using EasyClaimsCore.API.Services.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace EasyClaimsCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        /// <summary>
        /// Get overall system analytics overview
        /// </summary>
        [HttpGet("overview")]
        public async Task<ActionResult<ApiResponse<object>>> GetOverview(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var overview = await _analyticsService.GetOverviewAsync(startDate, endDate);
            return Ok(ApiResponse<object>.CreateSuccess(overview));
        }

        /// <summary>
        /// Get API usage statistics by endpoint
        /// </summary>
        [HttpGet("api-usage")]
        public async Task<ActionResult<ApiResponse<object>>> GetApiUsage(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? hospitalId = null)
        {
            var usage = await _analyticsService.GetApiUsageAsync(startDate, endDate, hospitalId);
            return Ok(ApiResponse<object>.CreateSuccess(usage));
        }

        /// <summary>
        /// Get performance metrics and trends
        /// </summary>
        [HttpGet("performance")]
        public async Task<ActionResult<ApiResponse<object>>> GetPerformanceMetrics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? methodName = null)
        {
            var performance = await _analyticsService.GetPerformanceMetricsAsync(startDate, endDate, methodName);
            return Ok(ApiResponse<object>.CreateSuccess(performance));
        }

        /// <summary>
        /// Get hospital activity and usage patterns
        /// </summary>
        [HttpGet("hospitals")]
        public async Task<ActionResult<ApiResponse<object>>> GetHospitalAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var hospitalStats = await _analyticsService.GetHospitalAnalyticsAsync(startDate, endDate);
            return Ok(ApiResponse<object>.CreateSuccess(hospitalStats));
        }

        /// <summary>
        /// Get error analysis and failure patterns
        /// </summary>
        [HttpGet("errors")]
        public async Task<ActionResult<ApiResponse<object>>> GetErrorAnalysis(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? hospitalId = null)
        {
            var errorStats = await _analyticsService.GetErrorAnalysisAsync(startDate, endDate, hospitalId);
            return Ok(ApiResponse<object>.CreateSuccess(errorStats));
        }

        /// <summary>
        /// Get real-time activity feed
        /// </summary>
        [HttpGet("activity")]
        public async Task<ActionResult<ApiResponse<object>>> GetRecentActivity(
            [FromQuery] int limit = 50,
            [FromQuery] string? status = null)
        {
            var activity = await _analyticsService.GetRecentActivityAsync(limit, status);
            return Ok(ApiResponse<object>.CreateSuccess(activity));
        }

        /// <summary>
        /// Get time-based trends and patterns
        /// </summary>
        [HttpGet("trends")]
        public async Task<ActionResult<ApiResponse<object>>> GetTrends(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string groupBy = "hour")
        {
            var trends = await _analyticsService.GetTrendsAsync(startDate, endDate, groupBy);
            return Ok(ApiResponse<object>.CreateSuccess(trends));
        }

        /// <summary>
        /// Export analytics data
        /// </summary>
        [HttpGet("export")]
        public async Task<ActionResult> ExportData(
            [FromQuery] string format = "csv",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var exportData = await _analyticsService.ExportDataAsync(startDate, endDate, format);

            var contentType = format.ToLower() switch
            {
                "json" => "application/json",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

            var fileName = $"analytics_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{format}";
            return File(exportData, contentType, fileName);
        }

        /// <summary>
        /// Get dashboard page
        /// </summary>
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            return Content(GetDashboardHtml(), "text/html");
        }

        private string GetDashboardHtml()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>EasyClaims Analytics Dashboard</title>
    <script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
    <script src=""https://cdn.tailwindcss.com""></script>
    <link href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css"" rel=""stylesheet"">
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');
        
        body { 
            font-family: 'Inter', sans-serif; 
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
        }
        
        .glass-effect {
            background: rgba(255, 255, 255, 0.25);
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.2);
        }
        
        .card-hover {
            transition: all 0.3s ease;
        }
        
        .card-hover:hover {
            transform: translateY(-2px);
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
        }
        
        .pulse-animation {
            animation: pulse 2s infinite;
        }
        
        @keyframes pulse {
            0%, 100% { opacity: 1; }
            50% { opacity: 0.5; }
        }
        
        .loading-skeleton {
            background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
            background-size: 200% 100%;
            animation: loading 1.5s infinite;
        }
        
        @keyframes loading {
            0% { background-position: 200% 0; }
            100% { background-position: -200% 0; }
        }
        
        .status-indicator {
            position: relative;
        }
        
        .status-indicator::before {
            content: '';
            position: absolute;
            top: 50%;
            left: -10px;
            width: 6px;
            height: 6px;
            border-radius: 50%;
            transform: translateY(-50%);
        }
        
        .status-success::before { background-color: #10B981; }
        .status-error::before { background-color: #EF4444; }
        .status-failed::before { background-color: #EF4444; }
        .status-pending::before { background-color: #F59E0B; }
        .status-unknown::before { background-color: #6B7280; }
    </style>
</head>
<body class=""min-h-screen"">
    <!-- Header -->
    <header class=""glass-effect border-b border-white/20 sticky top-0 z-50"">
        <div class=""max-w-7xl mx-auto px-4 sm:px-6 lg:px-8"">
            <div class=""flex justify-between items-center h-16"">
                <div class=""flex items-center"">
                    <div class=""flex items-center"">
                        <i class=""fas fa-chart-line text-2xl text-white mr-3""></i>
                        <h1 class=""text-2xl font-bold text-white"">EasyClaims Analytics</h1>
                    </div>
                    <div id=""connectionStatus"" class=""ml-6 px-3 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800"">
                        <i class=""fas fa-circle text-green-500 mr-1""></i>
                        Connected
                    </div>
                </div>
                <div class=""flex items-center space-x-4"">
                    <select id=""dateRange"" class=""bg-white/20 backdrop-blur border border-white/30 rounded-md px-3 py-2 text-sm text-white placeholder-white/70"">
                        <option value=""today"">Today</option>
                        <option value=""week"" selected>Last 7 Days</option>
                        <option value=""month"">Last 30 Days</option>
                        <option value=""quarter"">Last 90 Days</option>
                    </select>
                    <button onclick=""loadDashboardData()"" id=""refreshBtn"" class=""bg-white/20 backdrop-blur hover:bg-white/30 text-white px-4 py-2 rounded-md text-sm font-medium border border-white/30 transition-all duration-200"">
                        <i class=""fas fa-sync-alt mr-2""></i>Refresh
                    </button>
                    <button onclick=""exportData()"" class=""bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium transition-all duration-200"">
                        <i class=""fas fa-download mr-2""></i>Export
                    </button>
                </div>
            </div>
        </div>
    </header>

    <!-- Main Content -->
    <main class=""max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8"">
        <!-- Overview Cards -->
        <div class=""grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8"">
            <!-- Total API Calls -->
            <div class=""bg-white rounded-xl shadow-lg p-6 card-hover"">
                <div class=""flex items-center justify-between"">
                    <div>
                        <p class=""text-sm font-medium text-gray-600"">Total API Calls</p>
                        <p id=""totalCalls"" class=""text-3xl font-bold text-gray-900 mt-2"">-</p>
                        <p id=""totalCallsChange"" class=""text-sm text-gray-500 mt-1"">vs previous period</p>
                    </div>
                    <div class=""p-3 bg-blue-100 rounded-full"">
                        <i class=""fas fa-chart-line text-xl text-blue-600""></i>
                    </div>
                </div>
                <div class=""mt-4"">
                    <div class=""w-full bg-gray-200 rounded-full h-2"">
                        <div id=""totalCallsProgress"" class=""bg-blue-600 h-2 rounded-full transition-all duration-500"" style=""width: 0%""></div>
                    </div>
                </div>
            </div>
            
            <!-- Success Rate -->
            <div class=""bg-white rounded-xl shadow-lg p-6 card-hover"">
                <div class=""flex items-center justify-between"">
                    <div>
                        <p class=""text-sm font-medium text-gray-600"">Success Rate</p>
                        <p id=""successRate"" class=""text-3xl font-bold text-gray-900 mt-2"">-</p>
                        <p id=""successRateChange"" class=""text-sm text-gray-500 mt-1"">vs previous period</p>
                    </div>
                    <div class=""p-3 bg-green-100 rounded-full"">
                        <i class=""fas fa-check-circle text-xl text-green-600""></i>
                    </div>
                </div>
                <div class=""mt-4"">
                    <div class=""w-full bg-gray-200 rounded-full h-2"">
                        <div id=""successRateProgress"" class=""bg-green-600 h-2 rounded-full transition-all duration-500"" style=""width: 0%""></div>
                    </div>
                </div>
            </div>
            
            <!-- Average Response Time -->
            <div class=""bg-white rounded-xl shadow-lg p-6 card-hover"">
                <div class=""flex items-center justify-between"">
                    <div>
                        <p class=""text-sm font-medium text-gray-600"">Avg Response Time</p>
                        <p id=""avgResponseTime"" class=""text-3xl font-bold text-gray-900 mt-2"">-</p>
                        <p id=""avgResponseTimeChange"" class=""text-sm text-gray-500 mt-1"">vs previous period</p>
                    </div>
                    <div class=""p-3 bg-yellow-100 rounded-full"">
                        <i class=""fas fa-clock text-xl text-yellow-600""></i>
                    </div>
                </div>
                <div class=""mt-4"">
                    <div class=""w-full bg-gray-200 rounded-full h-2"">
                        <div id=""responseTimeProgress"" class=""bg-yellow-600 h-2 rounded-full transition-all duration-500"" style=""width: 0%""></div>
                    </div>
                </div>
            </div>
            
            <!-- Active Hospitals -->
            <div class=""bg-white rounded-xl shadow-lg p-6 card-hover"">
                <div class=""flex items-center justify-between"">
                    <div>
                        <p class=""text-sm font-medium text-gray-600"">Active Hospitals</p>
                        <p id=""activeHospitals"" class=""text-3xl font-bold text-gray-900 mt-2"">-</p>
                        <p id=""activeHospitalsChange"" class=""text-sm text-gray-500 mt-1"">vs previous period</p>
                    </div>
                    <div class=""p-3 bg-purple-100 rounded-full"">
                        <i class=""fas fa-hospital text-xl text-purple-600""></i>
                    </div>
                </div>
                <div class=""mt-4"">
                    <div class=""w-full bg-gray-200 rounded-full h-2"">
                        <div id=""hospitalsProgress"" class=""bg-purple-600 h-2 rounded-full transition-all duration-500"" style=""width: 0%""></div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Charts Row -->
        <div class=""grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8"">
            <!-- API Usage Chart -->
            <div class=""bg-white rounded-xl shadow-lg p-6 card-hover"">
                <div class=""flex items-center justify-between mb-6"">
                    <h3 class=""text-lg font-semibold text-gray-900"">API Usage by Endpoint</h3>
                    <div class=""flex space-x-2"">
                        <button onclick=""toggleChartType('apiUsage')"" class=""text-sm text-gray-500 hover:text-gray-700"">
                            <i class=""fas fa-chart-pie""></i>
                        </button>
                        <button class=""text-sm text-gray-500 hover:text-gray-700"">
                            <i class=""fas fa-download""></i>
                        </button>
                    </div>
                </div>
                <div class=""relative h-80"">
                    <canvas id=""apiUsageChart""></canvas>
                </div>
            </div>
            
            <!-- Trends Chart -->
            <div class=""bg-white rounded-xl shadow-lg p-6 card-hover"">
                <div class=""flex items-center justify-between mb-6"">
                    <h3 class=""text-lg font-semibold text-gray-900"">Usage Trends</h3>
                    <div class=""flex space-x-2"">
                        <select id=""trendsGroupBy"" class=""text-sm border border-gray-300 rounded px-2 py-1"" onchange=""updateTrendsChart()"">
                            <option value=""hour"">Hourly</option>
                            <option value=""day"">Daily</option>
                        </select>
                    </div>
                </div>
                <div class=""relative h-80"">
                    <canvas id=""trendsChart""></canvas>
                </div>
            </div>
        </div>

        <!-- Performance and Errors Row -->
        <div class=""grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8"">
            <!-- Performance Distribution -->
            <div class=""bg-white rounded-xl shadow-lg p-6 card-hover"">
                <div class=""flex items-center justify-between mb-6"">
                    <h3 class=""text-lg font-semibold text-gray-900"">Response Time Distribution</h3>
                </div>
                <div class=""relative h-64"">
                    <canvas id=""performanceChart""></canvas>
                </div>
            </div>
            
            <!-- Error Analysis -->
            <div class=""bg-white rounded-xl shadow-lg p-6 card-hover"">
                <div class=""flex items-center justify-between mb-6"">
                    <h3 class=""text-lg font-semibold text-gray-900"">Error Analysis</h3>
                    <span id=""errorRate"" class=""px-3 py-1 bg-red-100 text-red-800 rounded-full text-sm font-medium"">0% Error Rate</span>
                </div>
                <div id=""errorCategories"" class=""space-y-3"">
                    <!-- Error categories will be populated here -->
                </div>
            </div>
        </div>

        <!-- Tables Row -->
        <div class=""grid grid-cols-1 lg:grid-cols-2 gap-6"">
            <!-- Recent Activity -->
            <div class=""bg-white rounded-xl shadow-lg card-hover"">
                <div class=""px-6 py-4 border-b border-gray-200 flex items-center justify-between"">
                    <h3 class=""text-lg font-semibold text-gray-900"">Recent Activity</h3>
                    <div class=""flex items-center space-x-2"">
                        <select id=""activityFilter"" class=""text-sm border border-gray-300 rounded px-2 py-1"" onchange=""loadRecentActivity()"">
                            <option value="""">All Status</option>
                            <option value=""Success"">Success Only</option>
                            <option value=""Failed"">Failed Only</option>
                        </select>
                        <div id=""realTimeIndicator"" class=""w-2 h-2 bg-green-500 rounded-full pulse-animation""></div>
                    </div>
                </div>
                <div class=""p-6"">
                    <div id=""recentActivity"" class=""space-y-3 max-h-96 overflow-y-auto"">
                        <!-- Activity items will be inserted here -->
                    </div>
                </div>
            </div>
            
            <!-- Top Hospitals -->
            <div class=""bg-white rounded-xl shadow-lg card-hover"">
                <div class=""px-6 py-4 border-b border-gray-200"">
                    <h3 class=""text-lg font-semibold text-gray-900"">Top Hospitals</h3>
                </div>
                <div class=""p-6"">
                    <div id=""topHospitals"" class=""space-y-4 max-h-96 overflow-y-auto"">
                        <!-- Hospital stats will be inserted here -->
                    </div>
                </div>
            </div>
        </div>
    </main>

    <!-- Loading Overlay -->
    <div id=""loadingOverlay"" class=""fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center hidden"">
        <div class=""bg-white rounded-lg p-6 flex items-center space-x-3"">
            <div class=""animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600""></div>
            <span class=""text-gray-700"">Loading dashboard data...</span>
        </div>
    </div>

    <!-- Notification Container -->
    <div id=""notificationContainer"" class=""fixed top-4 right-4 z-50 space-y-2""></div>

    <script>
        // Global variables
        const API_BASE = '/api/analytics';
        let apiUsageChart, trendsChart, performanceChart;
        let currentChartTypes = { apiUsage: 'doughnut' };
        
        // Initialize dashboard
        document.addEventListener('DOMContentLoaded', function() {
            initializeDashboard();
        });
        
        async function initializeDashboard() {
            try {
                showLoading(true);
                await loadDashboardData();
                setupAutoRefresh();
                showNotification('Dashboard loaded successfully', 'success');
            } catch (error) {
                console.error('Failed to initialize dashboard:', error);
                showNotification('Failed to load dashboard', 'error');
            } finally {
                showLoading(false);
            }
        }
        
        async function loadDashboardData() {
            try {
                updateConnectionStatus('connecting');
                
                const dateRange = document.getElementById('dateRange').value;
                const dates = getDateRange(dateRange);
                
                console.log('Loading dashboard with date range:', dateRange, dates); // DEBUG LOG
                
                // Show loading state
                showLoadingSkeletons(true);
                
                // Build URLs with debug logging
                const urls = {
                    overview: `${API_BASE}/overview?startDate=${dates.start}&endDate=${dates.end}`,
                    apiUsage: `${API_BASE}/api-usage?startDate=${dates.start}&endDate=${dates.end}`,
                    performance: `${API_BASE}/performance?startDate=${dates.start}&endDate=${dates.end}`,
                    trends: `${API_BASE}/trends?startDate=${dates.start}&endDate=${dates.end}&groupBy=hour`,
                    activity: `${API_BASE}/activity?limit=15`,
                    hospitals: `${API_BASE}/hospitals?startDate=${dates.start}&endDate=${dates.end}`,
                    errors: `${API_BASE}/errors?startDate=${dates.start}&endDate=${dates.end}`
                };
                
                console.log('API URLs being called:', urls); // DEBUG LOG
                
                // Load all data concurrently
                const [overview, apiUsage, performance, trends, activity, hospitals, errors] = await Promise.all([
                    fetchWithRetry(urls.overview),
                    fetchWithRetry(urls.apiUsage),
                    fetchWithRetry(urls.performance),
                    fetchWithRetry(urls.trends),
                    fetchWithRetry(urls.activity),
                    fetchWithRetry(urls.hospitals),
                    fetchWithRetry(urls.errors)
                ]);
                
                console.log('API Responses received:', { 
                    overview: overview?.data, 
                    apiUsage: apiUsage?.data?.length, 
                    activity: activity?.data?.length,
                    hospitals: hospitals?.data?.length 
                }); // DEBUG LOG
                
                // Update all components - handle API response structure
                updateOverviewCards(overview?.data || {});
                updateApiUsageChart(apiUsage?.data || []);
                updatePerformanceChart(performance?.data || {});
                updateTrendsChart(trends?.data || []);
                updateRecentActivity(activity?.data || []);
                updateTopHospitals(hospitals?.data || []);
                updateErrorAnalysis(errors?.data || {});
                
                updateConnectionStatus('connected');
                showLoadingSkeletons(false);
                
            } catch (error) {
                console.error('Error loading dashboard data:', error);
                updateConnectionStatus('error');
                showNotification('Failed to load dashboard data', 'error');
                showLoadingSkeletons(false);
            }
        }
        
        async function fetchWithRetry(url, retries = 3) {
            for (let i = 0; i < retries; i++) {
                try {
                    const response = await fetch(url);
                    if (!response.ok) throw new Error(`HTTP ${response.status}`);
                    return await response.json();
                } catch (error) {
                    if (i === retries - 1) throw error;
                    await new Promise(resolve => setTimeout(resolve, 1000 * (i + 1)));
                }
            }
        }
        
        function getDateRange(range) {
            const end = new Date().toISOString();
            let start;
            
            switch(range) {
                case 'today':
                    start = new Date();
                    start.setHours(0, 0, 0, 0);
                    break;
                case 'week':
                    start = new Date();
                    start.setDate(start.getDate() - 7);
                    break;
                case 'month':
                    start = new Date();
                    start.setDate(start.getDate() - 30);
                    break;
                case 'quarter':
                    start = new Date();
                    start.setDate(start.getDate() - 90);
                    break;
                default:
                    start = new Date();
                    start.setDate(start.getDate() - 7);
            }
            
            const result = { start: start.toISOString(), end: end };
            console.log('Date range calculation:', { range, result }); // DEBUG LOG
            return result;
        }
        
        function updateOverviewCards(data) {
            // Safely handle potentially undefined data
            if (!data) {
                console.warn('No overview data received');
                return;
            }
            
            // Animate numbers
            animateNumber('totalCalls', data.totalCalls || 0);
            animateNumber('activeHospitals', data.activeHospitals || 0);
            
            document.getElementById('successRate').textContent = data.successRate ? `${data.successRate.toFixed(1)}%` : '0%';
            document.getElementById('avgResponseTime').textContent = data.averageResponseTime ? `${Math.round(data.averageResponseTime)}ms` : '0ms';
            
            // Update progress bars
            updateProgressBar('totalCallsProgress', Math.min(100, (data.totalCalls || 0) / 100));
            updateProgressBar('successRateProgress', data.successRate || 0);
            updateProgressBar('responseTimeProgress', Math.min(100, (data.averageResponseTime || 0) / 50));
            updateProgressBar('hospitalsProgress', Math.min(100, (data.activeHospitals || 0) * 10));
        }
        
        function animateNumber(elementId, targetValue) {
            const element = document.getElementById(elementId);
            const startValue = parseInt(element.textContent.replace(/,/g, '')) || 0;
            const duration = 1000;
            const startTime = performance.now();
            
            function updateValue(currentTime) {
                const elapsed = currentTime - startTime;
                const progress = Math.min(elapsed / duration, 1);
                const currentValue = Math.floor(startValue + (targetValue - startValue) * progress);
                element.textContent = currentValue.toLocaleString();
                
                if (progress < 1) {
                    requestAnimationFrame(updateValue);
                }
            }
            
            requestAnimationFrame(updateValue);
        }
        
        function updateProgressBar(elementId, percentage) {
            const element = document.getElementById(elementId);
            setTimeout(() => {
                element.style.width = `${Math.min(percentage, 100)}%`;
            }, 100);
        }
        
        function updateApiUsageChart(data) {
            if (!data || !Array.isArray(data)) {
                console.warn('No API usage data received or invalid format');
                return;
            }
            
            const ctx = document.getElementById('apiUsageChart').getContext('2d');
            
            if (apiUsageChart) {
                apiUsageChart.destroy();
            }
            
            const chartType = currentChartTypes.apiUsage;
            
            apiUsageChart = new Chart(ctx, {
                type: chartType,
                data: {
                    labels: data.map(item => (item.methodName || 'Unknown').replace(/([A-Z])/g, ' $1').trim()),
                    datasets: [{
                        label: 'API Calls',
                        data: data.map(item => item.callCount || 0),
                        backgroundColor: [
                            '#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6',
                            '#06B6D4', '#84CC16', '#F97316', '#EC4899', '#6B7280'
                        ],
                        borderWidth: chartType === 'bar' ? 0 : 2,
                        borderColor: '#fff'
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: chartType === 'doughnut' ? 'bottom' : 'top',
                            labels: { usePointStyle: true, padding: 20 }
                        },
                        tooltip: {
                            callbacks: {
                                label: function(context) {
                                    const item = data[context.dataIndex];
                                    if (!item) return context.label;
                                    return [
                                        `${context.label}: ${context.parsed}`,
                                        `Success Rate: ${(item.successRate || 0).toFixed(1)}%`,
                                        `Avg Response: ${(item.averageResponseTime || 0).toFixed(0)}ms`
                                    ];
                                }
                            }
                        }
                    },
                    scales: chartType === 'bar' ? {
                        y: { beginAtZero: true }
                    } : {}
                }
            });
        }
        
        function updateTrendsChart(data) {
            if (!data || !Array.isArray(data)) {
                console.warn('No trends data received or invalid format');
                return;
            }
            
            const ctx = document.getElementById('trendsChart').getContext('2d');
            
            if (trendsChart) {
                trendsChart.destroy();
            }
            
            trendsChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: data.map(item => {
                        const date = new Date(item.timestamp);
                        return date.toLocaleString('en-US', { 
                            month: 'short', 
                            day: 'numeric', 
                            hour: 'numeric' 
                        });
                    }),
                    datasets: [
                        {
                            label: 'Total Calls',
                            data: data.map(item => item.callCount || 0),
                            borderColor: '#3B82F6',
                            backgroundColor: 'rgba(59, 130, 246, 0.1)',
                            tension: 0.4,
                            fill: true
                        },
                        {
                            label: 'Success Count',
                            data: data.map(item => item.successCount || 0),
                            borderColor: '#10B981',
                            backgroundColor: 'rgba(16, 185, 129, 0.1)',
                            tension: 0.4,
                            yAxisID: 'y1'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    interaction: { intersect: false, mode: 'index' },
                    scales: {
                        y: { 
                            beginAtZero: true,
                            position: 'left'
                        },
                        y1: {
                            type: 'linear',
                            display: true,
                            position: 'right',
                            grid: { drawOnChartArea: false }
                        }
                    },
                    plugins: {
                        legend: { position: 'top' }
                    }
                }
            });
        }
        
        function updatePerformanceChart(data) {
            if (!data) {
                console.warn('No performance data received');
                return;
            }
            
            const ctx = document.getElementById('performanceChart');
            if (!ctx) return;
            
            const context = ctx.getContext('2d');
            
            if (performanceChart) {
                performanceChart.destroy();
            }
            
            const distribution = data.responseTimeDistribution || [];
            
            performanceChart = new Chart(context, {
                type: 'bar',
                data: {
                    labels: distribution.map(d => d.range || 'Unknown'),
                    datasets: [{
                        label: 'Request Count',
                        data: distribution.map(d => d.count || 0),
                        backgroundColor: '#F59E0B',
                        borderRadius: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { display: false }
                    },
                    scales: {
                        y: { beginAtZero: true }
                    }
                }
            });
        }
        
        function updateRecentActivity(data) {
            const container = document.getElementById('recentActivity');
            
            if (!data || !Array.isArray(data) || data.length === 0) {
                container.innerHTML = '<p class=""text-gray-500 text-center py-4"">No recent activity</p>';
                return;
            }
            
            container.innerHTML = data.map(item => `
                <div class=""flex items-center justify-between p-3 border border-gray-100 rounded-lg hover:bg-gray-50 transition-colors"">
                    <div class=""flex items-center space-x-3"">
                        <div class=""status-indicator status-${(item.status || 'unknown').toLowerCase()}"">
                            <i class=""fas fa-${item.status === 'Success' ? 'check' : 'times'} text-sm""></i>
                        </div>
                        <div>
                            <p class=""text-sm font-medium text-gray-900"">${item.methodName || 'Unknown Method'}</p>
                            <p class=""text-xs text-gray-500"">${item.hospitalId || 'Unknown'} • ${formatTimestamp(item.requested)}</p>
                        </div>
                    </div>
                    <div class=""text-right"">
                        <span class=""inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                            item.status === 'Success' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                        }"">${item.status || 'Unknown'}</span>
                        ${item.responseTimeMs ? `<p class=""text-xs text-gray-500 mt-1"">${Math.round(item.responseTimeMs)}ms</p>` : ''}
                    </div>
                </div>
            `).join('');
        }
        
        function updateTopHospitals(data) {
            const container = document.getElementById('topHospitals');
            
            if (!data || !Array.isArray(data) || data.length === 0) {
                container.innerHTML = '<p class=""text-gray-500 text-center py-4"">No hospital data</p>';
                return;
            }
            
            container.innerHTML = data.slice(0, 10).map((item, index) => `
                <div class=""flex items-center justify-between p-3 border border-gray-100 rounded-lg hover:bg-gray-50 transition-colors"">
                    <div class=""flex items-center space-x-3"">
                        <span class=""flex items-center justify-center w-8 h-8 bg-blue-100 text-blue-800 text-sm font-medium rounded-full"">
                            ${index + 1}
                        </span>
                        <div>
                            <p class=""text-sm font-medium text-gray-900"">${item.hospitalId || 'Unknown'}</p>
                            <p class=""text-xs text-gray-500"">${item.totalCalls || 0} calls • ${(item.successRate || 0).toFixed(1)}% success</p>
                        </div>
                    </div>
                    <div class=""text-right"">
                        <div class=""w-16 bg-gray-200 rounded-full h-2"">
                            <div class=""bg-green-600 h-2 rounded-full"" style=""width: ${Math.min(100, item.successRate || 0)}%""></div>
                        </div>
                        <p class=""text-xs text-gray-500 mt-1"">${Math.round(item.averageResponseTime || 0)}ms avg</p>
                    </div>
                </div>
            `).join('');
        }
        
        function updateErrorAnalysis(data) {
            if (!data) {
                console.warn('No error analysis data received');
                return;
            }
            
            const errorRateElement = document.getElementById('errorRate');
            const categoriesContainer = document.getElementById('errorCategories');
            
            if (errorRateElement) {
                errorRateElement.textContent = `${(data.errorRate || 0).toFixed(1)}% Error Rate`;
                errorRateElement.className = `px-3 py-1 rounded-full text-sm font-medium ${
                    (data.errorRate || 0) > 5 ? 'bg-red-100 text-red-800' : 'bg-green-100 text-green-800'
                }`;
            }
            
            if (categoriesContainer) {
                if (!data.errorsByCategory || data.errorsByCategory.length === 0) {
                    categoriesContainer.innerHTML = '<p class=""text-gray-500 text-center py-4"">No errors to analyze</p>';
                    return;
                }
                
                categoriesContainer.innerHTML = data.errorsByCategory.map(category => `
                    <div class=""flex items-center justify-between"">
                        <div class=""flex items-center space-x-3"">
                            <div class=""w-3 h-3 bg-red-500 rounded-full""></div>
                            <span class=""text-sm text-gray-700"">${category.category || 'Unknown'}</span>
                        </div>
                        <div class=""flex items-center space-x-2"">
                            <span class=""text-sm font-medium text-gray-900"">${category.count || 0}</span>
                            <span class=""text-xs text-gray-500"">(${(category.percentage || 0).toFixed(1)}%)</span>
                        </div>
                    </div>
                `).join('');
            }
        }
        
        // Utility functions
        function formatTimestamp(timestamp) {
            const date = new Date(timestamp);
            const now = new Date();
            const diffMs = now - date;
            const diffMins = Math.floor(diffMs / 60000);
            const diffHours = Math.floor(diffMs / 3600000);
            
            if (diffMins < 1) return 'Just now';
            if (diffMins < 60) return `${diffMins}m ago`;
            if (diffHours < 24) return `${diffHours}h ago`;
            return date.toLocaleDateString();
        }
        
        function showLoading(show) {
            const overlay = document.getElementById('loadingOverlay');
            overlay.classList.toggle('hidden', !show);
        }
        
        function showLoadingSkeletons(show) {
            // Add/remove loading skeleton classes
            const elements = ['totalCalls', 'successRate', 'avgResponseTime', 'activeHospitals'];
            elements.forEach(id => {
                const element = document.getElementById(id);
                element.classList.toggle('loading-skeleton', show);
                if (show) element.textContent = '';
            });
        }
        
        function updateConnectionStatus(status) {
            const indicator = document.getElementById('connectionStatus');
            const statusConfig = {
                connected: { class: 'bg-green-100 text-green-800', icon: 'circle', text: 'Connected' },
                connecting: { class: 'bg-yellow-100 text-yellow-800', icon: 'circle', text: 'Connecting...' },
                error: { class: 'bg-red-100 text-red-800', icon: 'times-circle', text: 'Connection Error' }
            };
            
            const config = statusConfig[status] || statusConfig.error;
            indicator.className = `ml-6 px-3 py-1 rounded-full text-xs font-medium ${config.class}`;
            indicator.innerHTML = `<i class=""fas fa-${config.icon} mr-1""></i>${config.text}`;
        }
        
        function showNotification(message, type = 'info') {
            const container = document.getElementById('notificationContainer');
            const notification = document.createElement('div');
            
            const typeConfig = {
                success: 'bg-green-500',
                error: 'bg-red-500',
                info: 'bg-blue-500',
                warning: 'bg-yellow-500'
            };
            
            notification.className = `${typeConfig[type]} text-white px-4 py-2 rounded-md shadow-lg transform translate-x-full transition-transform duration-300`;
            notification.textContent = message;
            
            container.appendChild(notification);
            
            // Animate in
            setTimeout(() => {
                notification.classList.remove('translate-x-full');
            }, 100);
            
            // Remove after delay
            setTimeout(() => {
                notification.classList.add('translate-x-full');
                setTimeout(() => container.removeChild(notification), 300);
            }, 5000);
        }
        
        function toggleChartType(chartName) {
            if (chartName === 'apiUsage') {
                currentChartTypes.apiUsage = currentChartTypes.apiUsage === 'doughnut' ? 'bar' : 'doughnut';
                loadDashboardData(); // Reload to update chart
            }
        }
        
        function setupAutoRefresh() {
            // Refresh every 30 seconds
            setInterval(() => {
                loadDashboardData();
            }, 30000);
        }
        
        async function loadRecentActivity() {
            const filter = document.getElementById('activityFilter').value;
            const url = filter ? `${API_BASE}/activity?limit=15&status=${filter}` : `${API_BASE}/activity?limit=15`;
            
            try {
                const response = await fetch(url);
                const data = await response.json();
                updateRecentActivity(data.data);
            } catch (error) {
                console.error('Error loading recent activity:', error);
            }
        }
        
        async function updateTrendsChart() {
            const groupBy = document.getElementById('trendsGroupBy').value;
            const dateRange = document.getElementById('dateRange').value;
            const dates = getDateRange(dateRange);
            
            try {
                const response = await fetch(`${API_BASE}/trends?startDate=${dates.start}&endDate=${dates.end}&groupBy=${groupBy}`);
                const data = await response.json();
                updateTrendsChart(data.data);
            } catch (error) {
                console.error('Error updating trends chart:', error);
            }
        }
        
        async function exportData() {
            const dateRange = document.getElementById('dateRange').value;
            const dates = getDateRange(dateRange);
            
            try {
                showNotification('Preparing export...', 'info');
                const response = await fetch(`${API_BASE}/export?format=csv&startDate=${dates.start}&endDate=${dates.end}`);
                
                if (response.ok) {
                    const blob = await response.blob();
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = `easyclaims_analytics_${new Date().toISOString().split('T')[0]}.csv`;
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                    window.URL.revokeObjectURL(url);
                    showNotification('Export completed successfully', 'success');
                } else {
                    throw new Error('Export failed');
                }
            } catch (error) {
                console.error('Error exporting data:', error);
                showNotification('Export failed', 'error');
            }
        }
        
        // Event listeners
        document.getElementById('dateRange').addEventListener('change', loadDashboardData);
    </script>
</body>
</html>";
        }
    }
}