import json
import urllib.request
import urllib.error

url = "http://localhost:5046/api/Auth/register"

password = "AAAA1234@a"

# Campus coordinates for each university
campus_coords = {
    "University of Technology Sydney": (-33.8832, 151.2005),
    "University of Sydney": (-33.8886, 151.1873),
    "University of New South Wales": (-33.9173, 151.2313),
    "Macquarie University": (-33.7738, 151.1126),
    "Western University": (-33.8150, 151.0011),
}

mentors = [
    {"name": "Emma Janice", "university": "University of Technology Sydney",
     "about": "Passionate computer science student with experience in web development and programming. I love helping others learn and grow in their coding journey."},
    {"name": "Alex Chen", "university": "University of Sydney",
     "about": "Business student with internship experience at top consulting firms. I specialise in helping students develop professional skills and career planning."},
    {"name": "Sarah Williams", "university": "University of New South Wales",
     "about": "Law student passionate about justice and helping others understand legal concepts. I enjoy mentoring first-year students."},
    {"name": "Michael Rodriguez", "university": "University of Technology Sydney",
     "about": "IT professional with focus on cybersecurity. I help students understand complex technical concepts and prepare for industry."},
    {"name": "Danny Lim", "university": "University of Technology Sydney",
     "about": "IT professional with focus on cybersecurity. I help students understand complex technical concepts and prepare for industry."},
    {"name": "Brenden Yung", "university": "University of New South Wales",
     "about": "IT professional with focus on cybersecurity. I help students understand complex technical concepts and prepare for industry."},
    {"name": "Jennie Patel", "university": "Macquarie University",
     "about": "Aspiring engineer passionate about robotics and design thinking. I enjoy mentoring students on building real-world engineering projects."},
    {"name": "James O'Connor", "university": "University of Sydney",
     "about": "Communication student with experience in media projects. I like helping peers gain confidence in speaking and presenting ideas."},
    {"name": "Hannah Kim", "university": "University of New South Wales",
     "about": "Law student with a focus on contract law. I enjoy guiding others in building strong analytical skills and preparing for mooting competitions."},
    {"name": "Carlos Martinez", "university": "Western University",
     "about": "International Studies student interested in cultural exchange and policy-making. I mentor students on adapting to diverse environments."},
    {"name": "Emily Zhang", "university": "University of Sydney",
     "about": "Health sciences student passionate about improving community well-being. I mentor students who want to pursue careers in healthcare."},
    {"name": "Sung Jing Woo", "university": "University of Technology Sydney",
     "about": "Passionate software developer with expertise in C#, .NET, Python, and Linux. Dedicated to creating innovative solutions and mentoring others in technology and career development."},
    {"name": "Johnny Zhang", "university": "Macquarie University",
     "about": "Enthusiastic International Studies and Business student with a strong interest in global markets, cross-cultural communication and advertising. I enjoy mentoring peers on developing language skills, teamwork strategies and marketing projects that connect cultures and businesses."},
    {"name": "Victor Zhong", "university": "University of New South Wales",
     "about": "Sustainable-design enthusiast. Happy to help with studio crits, portfolio layout and CAD fundamentals."},
    {"name": "Hector Lim", "university": "University of Sydney",
     "about": "Patient explainer of tough proofs and problem-solving strategies. Can guide exam prep and LaTeX write-ups."},
    {"name": "David Lee", "university": "University of Technology Sydney",
     "about": "Focus on Asia-Pacific studies. I help with essay structure, research methods and presentation polish."},
    {"name": "Mia Su", "university": "Western University",
     "about": "Pre-service teacher passionate about inclusive learning. I can review lesson plans and share prac tips."},
    {"name": "Jay Kim", "university": "Western University",
     "about": "Pre-service teacher focused on inclusive learning. I can review lesson plans, share prac tips and discuss classroom strategies."},
]

students = [("Test", "Student"), ("Olivia", "Nguyen"), ("Liam", "Park")]


def register(payload):
    request = urllib.request.Request(
        url,
        data=json.dumps(payload).encode("utf-8"),
        headers={"Content-Type": "application/json"},
        method="POST",
    )
    try:
        with urllib.request.urlopen(request) as response:
            status = response.status
            body = response.read().decode("utf-8")
    except urllib.error.HTTPError as e:
        status = e.code
        body = e.read().decode("utf-8")

    try:
        message = json.loads(body).get("message")
    except (ValueError, AttributeError):
        message = body
    print(f"{payload['FirstName']} {payload['LastName']} ({status}): {message}")

# Mentors (Role 1)
for i, mentor in enumerate(mentors):
    firstName, lastName = mentor["name"].split(" ", 1)
    lat, lng = campus_coords[mentor["university"]]

    # Small offset so mentors at the same campus don't stack on one map pin
    offset = (i % 5) * 0.0015

    register({
        "Email": f"{firstName}-{lastName.replace(' ', '-')}@edumap.com",
        "Password": password,
        "FirstName": firstName,
        "LastName": lastName,
        "Role": 1,
        "About": mentor["about"],
        "Latitude": lat + offset,
        "Longitude": lng + offset,
    })

# Students (Role 0)
for firstName, lastName in students:
    register({
        "Email": f"{firstName}-{lastName}@edumap.com",
        "Password": password,
        "FirstName": firstName,
        "LastName": lastName,
        "Role": 0,
    })