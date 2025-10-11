import os
import json
import requests
import datetime

mentors = [
    {
        "id": 1,
        "name": "Emma Janice",
        "studies": "3rd Year, Computer Science",
        "university": "University of Technology Sydney",
        "skills": ["HTML", "Java", "JavaScript", "Python", "ReactJS"],
        "rating": 4,
        "reviews": 3,
        "about": "Passionate computer science student with experience in web development and programming. I love helping others learn and grow in their coding journey.",
        "image": "👩‍💻"
    },
    {
        "id": 2,
        "name": "Alex Chen",
        "studies": "4th Year, Business",
        "university": "University of Sydney",
        "skills": ["Marketing", "Finance", "Strategy", "Leadership"],
        "rating": 5,
        "reviews": 7,
        "about": "Business student with internship experience at top consulting firms. I specialise in helping students develop professional skills and career planning.",
        "image": "👨‍💼"
    },
    {
        "id": 3,
        "name": "Sarah Williams",
        "studies": "3rd Year, Law and Business",
        "university": "University of New South Wales",
        "skills": ["Contract Law", "Criminal Law", "Legal Research", "Marketing"],
        "rating": 4,
        "reviews": 5,
        "about": "Law student passionate about justice and helping others understand legal concepts. I enjoy mentoring first-year students.",
        "image": "👩‍⚖️"
    },
    {
        "id": 4,
        "name": "Michael Rodriguez",
        "studies": "4th, Information Technology",
        "university": "University of Technology Sydney",
        "skills": ["Cybersecurity", "Networking", "Database", "Cloud Computing"],
        "rating": 5,
        "reviews": 8,
        "about": "IT professional with focus on cybersecurity. I help students understand complex technical concepts and prepare for industry.",
        "image": "👨‍💻"
    },
    {
        "id": 5,
        "name": "Danny Lim",
        "studies": "5th/Final Year, Engineering",
        "university": "University of Technology Sydney",
        "skills": ["Cybersecurity", "Networking", "Database", "Cloud Computing"],
        "rating": 5,
        "reviews": 8,
        "about": "IT professional with focus on cybersecurity. I help students understand complex technical concepts and prepare for industry.",
        "image": "👨‍💼"
    },
    {
        "id": 6,
        "name": "Brenden Yung",
        "studies": "4th, Information Technology and Business",
        "university": "University of New South Wales",
        "skills": ["Cybersecurity", "Networking", "Database", "Cloud Computing"],
        "rating": 5,
        "reviews": 8,
        "about": "IT professional with focus on cybersecurity. I help students understand complex technical concepts and prepare for industry.",
        "image": "👨‍💻"
    },
    {
        "id": 7,
        "name": "Jennie Patel",
        "studies": "3rd Year, Engineering",
        "university": "Macquarie University",
        "skills": ["Mechanical Design", "CAD", "Robotics", "Problem-Solving"],
        "rating": 4,
        "reviews": 7,
        "about": "Aspiring engineer passionate about robotics and design thinking. I enjoy mentoring students on building real-world engineering projects.",
        "image": "👨‍💼"
    },
    {
        "id": 8,
        "name": "James O'Connor",
        "studies": "3rd Year, Communication",
        "university": "University of Sydney",
        "skills": ["Public Speaking", "Media Writing", "Marketing", "Teamwork"],
        "rating": 3,
        "reviews": 4,
        "about": "Communication student with experience in media projects. I like helping peers gain confidence in speaking and presenting ideas.",
        "image": "👨‍🎓"
    },
    {
        "id": 9,
        "name": "Hannah Kim",
        "studies": "5th Year, Law",
        "university": "University of New South Wales",
        "skills": ["Legal Research", "Contract Law", "Critical Thinking", "Mooting"],
        "rating": 5,
        "reviews": 9,
        "about": "Law student with a focus on contract law. I enjoy guiding others in building strong analytical skills and preparing for mooting competitions.",
        "image": "👩‍⚖️"
    },
    {
        "id": 10,
        "name": "Carlos Martinez",
        "studies": "3rd Year, International Studies",
        "university": "Western University",
        "skills": ["Global Politics", "Cultural Awareness", "Foreign Policy", "Research"],
        "rating": 4,
        "reviews": 5,
        "about": "International Studies student interested in cultural exchange and policy-making. I mentor students on adapting to diverse environments.",
        "image": "👨‍🎓"
    },
    {
        "id": 11,
        "name": "Emily Zhang",
        "studies": "4th Year, Health Sciences",
        "university": "University of Sydney",
        "skills": ["Anatomy", "Research", "Healthcare Systems", "Community Outreach"],
        "rating": 5,
        "reviews": 7,
        "about": "Health sciences student passionate about improving community well-being. I mentor students who want to pursue careers in healthcare.",
        "image": "👩‍⚕️"
    },
    {
        "id": 12,
        "name": "Sung Jing Woo",
        "studies": "3rd Year, Information Technology",
        "university": "University of Technology Sydney",
        "skills": ["C#", ".Net", "Python", "Linux"],
        "rating": 5,
        "reviews": 9,
        "about": "Passionate software developer with expertise in C#, .NET, Python, and Linux. Dedicated to creating innovative solutions and mentoring others in technology and career development.",
        "image": "👨‍💻"
    },
    {
        "id": 13,
        "name": "Johnny Zhang",
        "studies": "3rd Year, International Studies & Business",
        "university": "Macquarie University",
        "skills": ["Marketing", "Japanese", "Teamwork", "Advertising"],
        "rating": 5,
        "reviews": 8,
        "about": "Enthusiastic International Studies and Business student with a strong interest in global markets, cross-cultural communication and advertising. I enjoy mentoring peers on developing language skills, teamwork strategies and marketing projects that connect cultures and businesses.",
        "image": "👨‍💼"
    },
    {
        "id": 14,
        "name": "Victor Zhong",
        "studies": "3rd Year, Architecture",
        "university": "University of New South Wales",
        "skills": ["SketchUp", "AutoCAD", "Revit", "Rhino 3D", "Portfolio Review"],
        "rating": 4,
        "reviews": 9,
        "about": "Sustainable-design enthusiast. Happy to help with studio crits, portfolio layout and CAD fundamentals.",
        "image": "👷🏻‍♂️"
    },
    {
        "id": 15,
        "name": "Hector Lim",
        "studies": "4th year, Mathematics",
        "university": "University of Sydney",
        "skills": ["Calculus", "Linear Algebra", "Probability", "LaTeX", "Python (NumPy)"],
        "rating": 5,
        "reviews": 9,
        "about": "Patient explainer of tough proofs and problem-solving strategies. Can guide exam prep and LaTeX write-ups.",
        "image": "🧑‍🎓"
    },
    {
        "id": 16,
        "name": "David Lee",
        "studies": "3rd year, International Studies",
        "university": "University of Technology Sydney",
        "skills": ["Academic Writing", "Policy Analysis", "Qualitative Research", "Presentation Skills", "Referencing"],
        "rating": 5,
        "reviews": 9,
        "about": "Focus on Asia-Pacific studies. I help with essay structure, research methods and presentation polish.",
        "image": "🧑‍💼"
    },
    {
        "id": 17,
        "name": "Mia Su",
        "studies": "4th year, Education",
        "university": "Western University",
        "skills": ["Lesson Planning", "Classroom Management", "Educational Psychology", "Literacy Tutoring", "APA Referencing"],
        "rating": 5,
        "reviews": 9,
        "about": "Pre-service teacher passionate about inclusive learning. I can review lesson plans and share prac tips.",
        "image": "👩‍🏫"
    },
    {
        "id": 18,
        "name": "Jay Kim",
        "studies": "4th year, Education",
        "university": "Western University",
        "skills": ["Lesson Planning", "Classroom Management", "Educational Psychology", "Literacy Tutoring", "APA Referencing"],
        "rating": 5,
        "reviews": 9,
        "about": "Pre-service teacher focused on inclusive learning. I can review lesson plans, share prac tips and discuss classroom strategies.",
        "image": "🧑‍🏫"
    }
]

url = "http://localhost:5046/api/Auth/register"

def register(email, password, firstName, lastName, role):
    payload = {
        "Email": email,
        "Password": password,
        "FirstName": firstName,
        "LastName": lastName,
        "Role": role
    }

    response = requests.post(url, json=payload)
    data = response.json()
    message = data.get("message")
    print(firstName + " " + lastName + ":", message)

# Loop through each key
for mentor in mentors:
    firstName = mentor["name"].split()[0]
    lastName = mentor["name"].split()[1]
    email = firstName + "-" + lastName + "@edumap.com"
    password = "AAAA1234@a"
    role = 1
    register(email, password, firstName, lastName, role)