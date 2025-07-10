
"use strict";

class TradingDashboard {
    constructor() {
        this.isConnected = false;
        this.isPaused = false;
        this.updateInterval = null;
        this.currencyPairData = new Map();
        this.initializePolling();
        this.bindEvents();
    }

    bindEvents() {
        const pauseBtn = document.getElementById('pauseBtn');
        const resumeBtn = document.getElementById('resumeBtn');

        pauseBtn?.addEventListener('click', () => {
            this.pauseSimulation();
        });

        resumeBtn?.addEventListener('click', () => {
            this.resumeSimulation();
        });
    }

    initializePolling() {
        this.fetchAndUpdate();

        this.updateInterval = setInterval(() => {
            if (!this.isPaused) {
                this.fetchAndUpdate();
            }
        }, 2000);

        this.showNotification("מחובר לשרת בהצלחה", "success");
    }

    async fetchAndUpdate() {
        try {
            const response = await fetch('/api/rates', {
                method: 'GET',
                cache: 'no-store'
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            console.log('Currency rates updated:', data.map(p => `${p.baseCurrencyAbbreviation}/${p.quoteCurrencyAbbreviation}: ${p.currentRate.toFixed(4)}`));

            this.updateTable(data);
            this.updateStats(data);
            this.updateLastUpdateTime();

        } catch (error) {
            console.error('Error fetching data:', error);
            this.showNotification("שגיאה בקבלת נתונים", "error");
        }
    }

    updateTable(data) {
        data.forEach(pair => {
            const row = document.querySelector(`tr[data-pair-id="${pair.pairId}"]`);
            if (!row) return;

            // Store previous data for comparison
            const previousData = this.currencyPairData.get(pair.pairId);
            this.currencyPairData.set(pair.pairId, pair);

            // Update current rate with animation
            const rateCell = row.querySelector('.current-rate');
            if (rateCell) {
                this.animateNumberChange(rateCell, pair.currentRate.toFixed(4));
            }

            // Update change
            const changeCell = row.querySelector('.change-cell');
            if (changeCell) {
                const changeSpan = changeCell.querySelector('span');
                const changeIcon = changeCell.querySelector('i');

                if (changeSpan && changeIcon) {
                    const changeValue = pair.change;
                    const changeText = changeValue >= 0 ? `+${changeValue.toFixed(4)}` : changeValue.toFixed(4);
                    changeSpan.textContent = changeText;

                    // Update change styling and icon
                    changeCell.className = 'change-cell';
                    if (changeValue > 0) {
                        changeCell.classList.add('change-positive');
                        changeIcon.className = 'fas fa-arrow-up change-icon';
                        this.flashRow(row, 'green');
                    } else if (changeValue < 0) {
                        changeCell.classList.add('change-negative');
                        changeIcon.className = 'fas fa-arrow-down change-icon';
                        this.flashRow(row, 'red');
                    } else {
                        changeCell.classList.add('change-neutral');
                        changeIcon.className = 'fas fa-minus change-icon';
                    }
                }
            }

            // Update percentage
            const percentBadge = row.querySelector('.percent-badge');
            if (percentBadge) {
                const percentValue = pair.changePercent;
                const percentText = percentValue >= 0 ? `+${percentValue.toFixed(2)}%` : `${percentValue.toFixed(2)}%`;
                percentBadge.textContent = percentText;

                percentBadge.className = 'percent-badge';
                if (percentValue > 0) {
                    percentBadge.classList.add('percent-positive');
                } else if (percentValue < 0) {
                    percentBadge.classList.add('percent-negative');
                } else {
                    percentBadge.classList.add('percent-neutral');
                }
            }

            // Update min/max values
            const minCell = row.querySelector('.min-value');
            const maxCell = row.querySelector('.max-value');
            if (minCell) minCell.textContent = pair.minValue.toFixed(4);
            if (maxCell) maxCell.textContent = pair.maxValue.toFixed(4);

            // Update timestamp
            const timestampCell = row.querySelector('.last-update');
            if (timestampCell) {
                timestampCell.textContent = new Date(pair.lastUpdate).toLocaleTimeString();
            }
        });
    }

    animateNumberChange(element, newValue) {
        if (!element) return;

        element.style.transform = 'scale(1.1)';
        element.style.transition = 'transform 0.2s ease';

        setTimeout(() => {
            element.textContent = newValue;
            element.style.transform = 'scale(1)';
        }, 100);
    }

    flashRow(row, color) {
        if (!row) return;

        row.classList.add(`flash-${color}`);
        setTimeout(() => {
            row.classList.remove(`flash-${color}`);
        }, 1000);
    }

    updateStats(data) {
        const totalPairs = data.length;
        const activePairs = data.filter(u => u.change !== 0).length;
        const positivePairs = data.filter(u => u.change > 0).length;

        this.animateNumberChange(document.getElementById('totalPairs'), totalPairs.toString());
        this.animateNumberChange(document.getElementById('activePairs'), activePairs.toString());
        this.animateNumberChange(document.getElementById('positivePairs'), positivePairs.toString());
    }

    updateLastUpdateTime() {
        const lastUpdateElement = document.getElementById('lastUpdateTime');
        if (lastUpdateElement) {
            lastUpdateElement.textContent = new Date().toLocaleTimeString();
        }
    }

    showNotification(message, type = 'success') {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.textContent = message;

        let container = document.getElementById('notifications');
        if (!container) {
            container = document.createElement('div');
            container.id = 'notifications';
            container.style.position = 'fixed';
            container.style.top = '20px';
            container.style.right = '20px';
            container.style.zIndex = '1000';
            document.body.appendChild(container);
        }

        container.appendChild(notification);

        setTimeout(() => {
            notification.style.animation = 'slideOut 0.3s ease-in forwards';
            setTimeout(() => {
                if (container.contains(notification)) {
                    container.removeChild(notification);
                }
            }, 300);
        }, 3000);
    }

    async pauseSimulation() {
        try {
            const response = await fetch('/api/stop', { method: 'POST' });
            if (response.ok) {
                this.isPaused = true;
                document.getElementById('pauseBtn').style.display = 'none';
                document.getElementById('resumeBtn').style.display = 'flex';

                const liveDot = document.querySelector('.live-dot');
                if (liveDot) {
                    liveDot.style.animationPlayState = 'paused';
                }

                this.showNotification("סימולציה הושהתה", "warning");
            }
        } catch (error) {
            console.error('Error pausing simulation:', error);
            this.showNotification("שגיאה בהשהיית הסימולציה", "error");
        }
    }

    async resumeSimulation() {
        try {
            const response = await fetch('/api/start', { method: 'POST' });
            if (response.ok) {
                this.isPaused = false;
                document.getElementById('pauseBtn').style.display = 'flex';
                document.getElementById('resumeBtn').style.display = 'none';

                const liveDot = document.querySelector('.live-dot');
                if (liveDot) {
                    liveDot.style.animationPlayState = 'running';
                }

                this.showNotification("סימולציה חודשה", "success");
            }
        } catch (error) {
            console.error('Error resuming simulation:', error);
            this.showNotification("שגיאה בחידוש הסימולציה", "error");
        }
    }

    destroy() {
        if (this.updateInterval) {
            clearInterval(this.updateInterval);
        }
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    console.log('Initializing Trading Dashboard...');
    window.tradingDashboard = new TradingDashboard();
});

// Cleanup on page unload
window.addEventListener('beforeunload', () => {
    if (window.tradingDashboard) {
        window.tradingDashboard.destroy();
    }
});
