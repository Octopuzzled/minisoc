document.addEventListener("DOMContentLoaded", () => {
    loadEvents(); 
    
    document.getElementById("filter-btn").addEventListener("click", () => {
        const level = document.getElementById("levels").value;
        const provider = document.getElementById("provider").value;
        const timestart = document.getElementById("timestart").value;
        const timeend = document.getElementById("timeend").value;
        loadEvents(level, provider,timestart, timeend);
    });
});

async function loadEvents(level, provider, timestart, timeend) {
    const params = new URLSearchParams();
    if (level) params.append("level", level);
    if (provider) params.append("provider", provider);
    if (timestart) params.append("startTime", new Date(timestart).toISOString());
    if (timeend) params.append("endTime", new Date(timeend).toISOString());
    const url = `http://localhost:5152/events?${params}`;
    const response = await fetch(url);
    const events = await response.json();
    const table = document.getElementById("events-body");
    table.innerHTML = "";
    for (const event of events) {
        const row = table.insertRow();
        row.insertCell().textContent = event.timestamp;
        row.insertCell().textContent = event.host;
        const levelCell = row.insertCell();
        levelCell.textContent = event.level;
        levelCell.className = `level-${event.level.toLowerCase()}`;
        row.insertCell().textContent = event.provider;
        row.insertCell().textContent = event.source;
        row.insertCell().textContent = event.message;
    }
}