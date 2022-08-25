const submitBtn = document.getElementById("submit");
const accessKey = document.getElementById("access-key")
const originalUrl = document.getElementById("original-url")
const shortLinkName = document.getElementById("short-link-name")
const resultLinksContainer = document.getElementById("result-links-container")

submitBtn.addEventListener("click", submit)
accessKey.focus()

async function submit() {
	if (!validate())
		return

	let customTags = document.querySelectorAll(".custom-tag")
	customTags = Array.from(customTags)
		.filter(x => x.value !== "")
		.map(x => ({index: x.name, value: x.value}))

	submitBtn.setAttribute("disabled", null)
	await createLink(customTags).catch(() => alert("Server is unavailable :( Ask your Dan :)"))
	submitBtn.removeAttribute("disabled")
}

async function createLink(customTags) {
	const body = {
		accessKey: accessKey.value,
		originalUrl: originalUrl.value,
		shortLinkName: shortLinkName.value,
		customTags: customTags
	}

	const response = await fetch("/api/link/create", {
		method: "POST",
		headers: {"Content-Type": "application/json"},
		body: JSON.stringify(body)
	})

	const content = await response.text()
	if (response.ok) {
		addResultLink(content)
	} else if (response.status === 401)
		alert("Invalid access key!")
	else if (response.status >= 500)
		alert("Internal server error :( Ask Dan to see logs")
	else
		alert(content)
}

const baseUrl = "https://a.apiexperts.ru"
function addResultLink(linkName) {
	const url = `${baseUrl}/${linkName}`
	const resultLink = document.createElement("h4")
	resultLink.innerHTML += `<a href="${url}">${url}</a>`
	resultLinksContainer.appendChild(resultLink)
}

function validate() {
	if (accessKey.value === "") {
		alert("Access key is empty!")
		return false
	}
	if (originalUrl.value === "") {
		alert("Original url is empty!")
		return false
	}
	return true
}

const createLinkTab = document.getElementById("create-link-tab")
const createLinkTabBtn = document.getElementById("create-link-tab-btn")
const exportTab = document.getElementById("export-tab")
const exportTabBtn = document.getElementById("export-tab-btn")

createLinkTabBtn.addEventListener("click", selectCreateLinkTab)
exportTabBtn.addEventListener("click", selectExportTab)
selectCreateLinkTab()

// selectExportTab()

function selectCreateLinkTab() {
	exportTab.className = "hidden"
	createLinkTabBtn.setAttribute("disabled", null)
	createLinkTab.className = ""
	exportTabBtn.removeAttribute("disabled")
}

function selectExportTab() {
	createLinkTab.className = "hidden"
	exportTabBtn.setAttribute("disabled", null)
	exportTab.className = ""
	createLinkTabBtn.removeAttribute("disabled")
}

const exportBtn = document.getElementById("export-btn")
const exportDateSince = document.getElementById("export-date-since")
const exportDateUntil = document.getElementById("export-date-until")
exportBtn.addEventListener("click", createExport)

async function createExport() {
	if (accessKey.value === "") {
		alert("Access key is empty!")
		return false
	}
	if (exportDateSince.value === "") {
		alert("Since date is empty!")
		return
	}
	if (exportDateUntil.value === "") {
		alert("Until date is empty!")
		return
	}

	const body = {
		since: exportDateSince.value,
		until: exportDateUntil.value,
		accessKey: accessKey.value
	}

	const response = await fetch("/api/export/create", {
		method: "POST",
		headers: {"Content-Type": "application/json"},
		body: JSON.stringify(body)
	})

	const content = await response.text();
	if (response.status === 401)
		alert("Invalid access key!")
	else if (!response.ok)
		alert(content)
	else
		window.open(content, "_blank")
}