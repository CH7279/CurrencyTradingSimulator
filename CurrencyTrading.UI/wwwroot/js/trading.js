"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/tradingHub") // מתחבר ל-Hub
    .build();

connection.on("RatesUpdated", function (updates) {
    // כאן אנחנו מקבלים את העדכונים ומעדכנים את התצוגה
    console.log("Received updates:", updates);
    // עדכון התצוגה (לדוג' עדכון טבלה או סטטיסטיקות)
    updateTable(updates);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

// פונקציה לעדכון נתונים בתצוגה
function updateTable(updates) {
    updates.forEach(update => {
        const row = document.querySelector(`tr[data-pair-id="${update.pairId}"]`);
        if (!row) return;

        const rateCell = row.querySelector('.current-rate');
        rateCell.textContent = update.newRate.toFixed(4); // עדכון שער חדש
    });
}
