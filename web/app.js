// Initialize event listeners after DOM is fully loaded
document.addEventListener("DOMContentLoaded", () => {
    loadEvents(); 
    loadMetrics();
    
    // Apply filters on button click
    document.getElementById("filter-btn").addEventListener("click", () => {
        const level = document.getElementById("levels").value;
        const provider = document.getElementById("provider").value;
        const timestart = document.getElementById("timestart").value;
        const timeend = document.getElementById("timeend").value;
        loadEvents(level, provider, timestart, timeend);
    });

    // Allow submitting filters with Enter key
    document.addEventListener("keydown", (e) => {
        if (e.key === "Enter") {
            document.getElementById("filter-btn").click();
        }
    });
});

// Fetches events from the API with optional filters and renders them in the table
async function loadEvents(level, provider, timestart, timeend) {
    const params = new URLSearchParams();
    if (level) params.append("level", level);
    if (provider) params.append("provider", provider);
    // Append :00Z to treat datetime-local input as UTC
    if (timestart) params.append("startTime", timestart + ":00Z");
    if (timeend) params.append("endTime", timeend + ":00Z");
    const url = `http://localhost:5152/events?${params}`;
    const response = await fetch(url);
    const events = await response.json();
    const table = document.getElementById("events-body");
    // Clear existing rows before rendering new results
    table.innerHTML = "";
    for (const event of events) {
        const row = table.insertRow();
        row.insertCell().textContent = event.timestamp;
        row.insertCell().textContent = event.host;
        const levelCell = row.insertCell();
        levelCell.textContent = event.level;
        // Apply color coding based on severity level
        levelCell.className = `level-${event.level.toLowerCase()}`;
        row.insertCell().textContent = event.provider;
        row.insertCell().textContent = event.source;
        row.insertCell().textContent = event.message;
    }
}

// Fetches aggregated metrics from the API and renders metric cards and charts
async function loadMetrics() {
    const response = await fetch('http://localhost:5152/metrics');
    const metrics = await response.json();

    // Update metric cards
    document.getElementById("card-total").textContent = metrics.event_count;
    document.getElementById("card-errors").textContent = metrics.by_level["Error"] ?? 0;
    document.getElementById("card-hosts").textContent = Object.keys(metrics.by_host).length;

    // By level chart
    const levelData = metrics.by_level;
    const llabels = Object.keys(levelData);
    const ldata = Object.values(levelData);

    // Map level names to consistent colors
    const levelColors = {
        "Error": "#ff6b6b",
        "Warning": "#ffa94d",
        "Information": "#74c0fc"
    };
    const colors = llabels.map(label => levelColors[label] ?? "#888");

    new Chart(document.getElementById("clevel"), {
        type: "bar",
        data: {
            labels: llabels,
            datasets: [{
                label: "Events by Level",
                data: ldata,
                backgroundColor: colors
            }]
        },
        options: {
            scales: {
                y: { ticks: { stepSize: 1 } }
            }
        }
    });

    // By host chart
    const hostData = metrics.by_host;
    const hlabels = Object.keys(hostData);
    const hdata = Object.values(hostData);

    new Chart(document.getElementById("chost"), {
        type: "bar",
        data: {
            labels: hlabels,
            datasets: [{
                label: "Events by Host",
                data: hdata,
                backgroundColor: "#69db7c"
            }]
        },
        options: {
            scales: {
                y: { ticks: { stepSize: 1 } }
            }
        }
    });

    // Last 24 hours trend chart (hourly buckets, time truncated to HH:MM)
    const hoursAllData = metrics.trend.last_24h;
    const hoursLabels = hoursAllData.map(bucket => bucket.time.substring(11, 16));
    const hoursData = hoursAllData.map(bucket => bucket.count);

    new Chart(document.getElementById("c24h"), {
        type: "line",
        data: {
            labels: hoursLabels,
            datasets: [{
                label: "Events of last 24 hours",
                data: hoursData,
                borderColor: "#ffa94d",
                backgroundColor: "transparent"
            }]
        },
        options: {
            scales: {
                y: { min: 0, ticks: { stepSize: 1 } }
            }
        }
    });

    // Last 7 days trend chart (daily buckets, time truncated to YYYY-MM-DD)
    const daysAllData = metrics.trend.last_7d;
    const daysLabels = daysAllData.map(bucket => bucket.time.substring(0, 10));
    const daysData = daysAllData.map(bucket => bucket.count);

    new Chart(document.getElementById("c7d"), {
        type: "line",
        data: {
            labels: daysLabels,
            datasets: [{
                label: "Events of last 7 days",
                data: daysData,
                borderColor: "#da77f2",
                backgroundColor: "transparent"
            }]
        },
        options: {
            scales: {
                y: { min: 0, ticks: { stepSize: 1 } }
            }
        }
    });
}