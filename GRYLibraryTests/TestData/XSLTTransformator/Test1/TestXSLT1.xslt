<xsl:stylesheet version="1.0"
 xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output omit-xml-declaration="yes" indent="yes"/>
    <xsl:strip-space elements="*"/>

    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="subObject">
        <xsl:copy>
            <xsl:value-of select="concat(property2, ',', property3,',', property4)"/>
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="subObject/node()"/>
</xsl:stylesheet>