# Post-Apo-Command-Unity

## 🎮 Project Overview
**Post-Apo-Command-Unity** is a strategic resource management demo built in **Unity**. Set **5 years after a global nuclear cataclysm**, players take on the role of a **Survivor Group Leader**. Your goal is to lead your people across a radioactive wasteland to a final extraction point signaled over a faint radio broadcast.

## 🧠 Leadership & Strategy Objectives
This project translates core management and decision-making skills into gameplay:
* **Dynamic Delegation:** Strategically forming teams of **1 to 3 survivors**. Leaders must decide whether to send a full squad for safety or a solo scout to conserve dwindling supplies.
* **Risk vs. Efficiency:** Balancing the **Fatigue System** against expedition needs. Pushing exhausted survivors increases the risk of severe injury or disappearance.
* **Resource Logistics:** Managing "Supplies" for daily consumption. Players must decide when to push forward or when to take a **Massive Detour (Skip)** to avoid lethal areas at the cost of high resource consumption.

## ⚙️ Core Mechanics
* **Proportional Success Logic:** Success is calculated by the ratio of the team's combined Stats (Strength, Perception, Agility) against Area Requirements. 
* **Fatigue Penalty:** Every mission adds exhaustion. High fatigue acts as a percentage-based multiplier that reduces the entire team’s Success Chance.
* **Detour System:** Players can choose to bypass dangerous areas. This "Skip" mechanic simulates traveling around a hazard, consuming significantly more supplies without gaining any resources.
* **Injury & Permanent Loss:** Low-success expeditions carry the heavy weight of survivors sustaining severe injuries or potentially being lost during the mission.

## 🛠️ Technical Details
* **Engine:** Unity 6000.3.1f1
* **Architecture:** Decoupled system using **ScriptableObjects** for Survivor Templates and Area Data, allowing for easy balancing without touching core code.
* **Visuals:** Custom character portraits and atmospheric area art designed to enhance immersion and role-clarity.

## 🤖 AI Tool Usage & Collaboration
This project was developed with a "Human-AI Co-pilot" approach:
* **ChatGPT:** Used for initial world-building, narrative conceptualization, and architectural planning of the game's core systems.
* **GitHub Copilot:** Integrated into the development workflow to assist in writing efficient C# scripts and implementing game systems.
* **Gemini:** Assisted in refining complex mathematical formulas for the Success Logic and the Fatigue System to ensure balanced gameplay.

## 🚀 How to Play
1. **Read the Signal:** Check your current day and the target day to reach the extraction point.
2. **Assemble the Team:** Select 1-3 survivors based on their **Job Titles** (e.g., Heavy Lifter, Wilderness Hunter).
3. **Analyze the Area:** Compare your team's total stats with the area's requirements.
4. **Decide:** Execute the mission to gain supplies or take a costly detour if the risk is too high.
5. **Manage Rest:** Ensure tired survivors stay back in the camp to recover for future challenges.
